using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


public class BeeStateClass
{
    //States the be can do
    public enum State { Idle, Traveling, Recolecting};

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
        animator = beeObject.GetComponent<Animator>();
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
    }


    //Traveling Behavior
    public override void Update()
    {
        Debug.Log(beeScript.targetFlower.name);

        //If the target flower is not null
        if (beeScript.targetFlower != null)
        {
            agent.SetDestination(beeScript.targetFlower.transform.position);
        }
            
    }


    //Exit the Traveling state and reset it
    public override void Exit()
    {
        base.Exit();
    }
}
