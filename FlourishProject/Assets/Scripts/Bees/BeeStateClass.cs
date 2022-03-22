using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


public class BeeStateClass
{
    //States the be can do
    public enum State { Idle, Traveling, Recollecting};

    //The phase the state is in, each state has 3 phases
    public enum Event { Enter, Update, Exit };


    //Public Variables
    public State stateName; //State it's in

    //Private Variables
    protected GameObject bee; //The Bee AI acts on
    protected BeeAiScript beeScript; //The Bee script
    protected NavMeshAgent agent;
    protected Animator animator;

    protected Event phase; //The current phase of the state;
    protected BeeStateClass nextState; //Next state to change to


    //Constructor of the class BeeStateClass
    public BeeStateClass(GameObject beeObject, BeeAiScript script)
    {
        bee = beeObject;
        beeScript = script;
        agent = beeObject.GetComponent<NavMeshAgent>();
        animator = beeScript.animator;
        phase = Event.Enter;
    }


    //Base processes of the phases that control each state
    public virtual void Enter()
    {
        phase = Event.Update;
    }

    public virtual void Update()
    {
        phase = Event.Update;
    }

    public virtual void Exit()
    {
        phase = Event.Exit;
    }


    //Launch a process to manage the phases of each state
    //calling the adequate phases depending on the current event.
    public BeeStateClass Process()
    {
        if (phase == Event.Enter) Enter();
        if (phase == Event.Update) Update();
        if (phase == Event.Exit)
        {
            Exit();
            return nextState;
        }

        return this;
    }


}


//Idle state
public class Idle : BeeStateClass
{
    //Idle's constructor (Take the bee from the base class)
    public Idle(GameObject beeObject, BeeAiScript script) : base(beeObject, script)
    {
        stateName = State.Idle;
        animator.SetBool("OnFlower", false);
    }


    //Enter the Idle
    public override void Enter()
    {
        base.Enter();
    }


    //Idle's behavior
    public override void Update()
    {
        //Enter the Traveling state immediately if the list of flowers has at least 1
        if (beeScript.listOfFlowers.Count > 0)
        {
            //Select a flower as a target
            int randomDestination = Random.Range(0, beeScript.listOfFlowers.Count);
            beeScript.targetFlower = beeScript.listOfFlowers[randomDestination];

            nextState = new Traveling(bee, beeScript);

            //Exit the Idle state
            phase = Event.Exit;
        }
    }


    //Exit the Idle state and reset it
    public override void Exit()
    {
        base.Exit();
    }
}


//Traveling state
public class Traveling : BeeStateClass
{
    //Traveling's constructor (Take the bee from the base class)
    public Traveling(GameObject beeObject, BeeAiScript script) : base(beeObject, script)
    {
        stateName = State.Traveling;
    }


    //Enter the Traveling
    public override void Enter()
    {
        base.Enter();
        animator.SetBool("OnFlower", false);
    }


    //Traveling Behavior
    public override void Update()
    {

        //If the target flower is not null
        if (beeScript.targetFlower != null)
        {
            agent.SetDestination(beeScript.targetFlower.transform.position);
        }

        //If the agent is in the target position, it doesn't have path or it's speed is 0, land in the flower
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //Using this because the agent when starting for the first time returns 0, so the stoppingDistance is 0.01f
            if (agent.remainingDistance <= 0f) return;

            //Using sqrMagnitude instead of magnitude increases performance
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                nextState = new Recollecting(bee, beeScript);

                //Exit the Traveling state
                phase = Event.Exit;
            }
        }

    }


    //Exit the Traveling state and reset it
    public override void Exit()
    {
        base.Exit();
    }
}


//Recollecting state
public class Recollecting : BeeStateClass
{
    //Recollecting's constructor (Take the bee from the base class)
    public Recollecting(GameObject beeObject, BeeAiScript script) : base(beeObject, script)
    {
        stateName = State.Recollecting;
    }


    //Enter the Recollecting
    public override void Enter()
    {
        base.Enter();
        animator.SetBool("OnFlower", true);
    }


    //Recollecting Behavior
    public override void Update()
    {
        Debug.Log("Recollecting");
    }


    //Exit the Recollecting state and reset it
    public override void Exit()
    {
        base.Exit();
    }
}
