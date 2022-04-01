//References
//https://stackoverflow.com/questions/61421172/why-does-navmeshagent-remainingdistance-return-values-of-infinity-and-then-a-flo


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using DG.Tweening;


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

    //Regulate the position, the rotation and the height of the bee
    public void RegulatePositionHeight()
    {

        //Regulate position
        beeScript.beeContainerObject.transform.position = new Vector3(
            agent.transform.position.x,
            beeScript.beeContainerObject.transform.position.y,
            agent.transform.position.z);

        //Regulate rotation
        beeScript.beeContainerObject.transform.rotation = agent.transform.rotation;

        //If not already tweening and the distance is less than infinite, regulate height
        if (!beeScript.alreadyTweening) //&& agent.remainingDistance < Mathf.Infinity)
        {
            beeScript.alreadyTweening = true;

            //float time = agent.remainingDistance / agent.speed;
            float time = GetAgentDistance(agent) / agent.speed;
            beeScript.beeContainerObject.transform.DOMoveY(beeScript.targetFlower.transform.position.y, time);
        }
    }


    //Get the navmesh distance with accuracy
    private float GetAgentDistance(NavMeshAgent navMeshAgent)
    {
        if (navMeshAgent.pathPending || navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || navMeshAgent.path.corners.Length == 0) return -1f;

        float distance = 0.0f;

        for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
        }

        return distance;
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
        animator.SetBool("OnFlower", false);
    }


    //Idle's behavior
    public override void Update()
    {
        //Enter the Traveling state immediately if the list of flowers has at least 1
        if (beeScript.listOfFlowers.Count > 0)
        {
            //If the bee had a target flower if coming from a State, set it as the previous flower 
            if (beeScript.targetFlower != null) beeScript.previousFlower = beeScript.targetFlower;

            //Select a flower as a target, if it's the same flower as the previous one, repeat
            do
            {
                int randomDestination = Random.Range(0, beeScript.listOfFlowers.Count);
                beeScript.targetFlower = beeScript.listOfFlowers[randomDestination];

            } while (beeScript.targetFlower == beeScript.previousFlower);

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
        beeScript.alreadyTweening = false;
    }


    //Traveling Behavior
    public override void Update()
    {

        //If the target flower is not null
        if (beeScript.targetFlower != null)
        {
            //Set the destination
            agent.SetDestination(beeScript.targetFlower.transform.position);

            //Regulate the position and the height if the distance is more than 0
            if (agent.remainingDistance > 0) RegulatePositionHeight();
        }
        else //if the flower has been deleted, choose another target
        {
            nextState = new Idle(bee, beeScript);

            //Exit the Traveling state
            phase = Event.Exit;
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
        beeScript.alreadyTweening = false;
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

        //The Coroutine is launched from the beeScript, normal classes cannot launch coroutines
        beeScript.StartCoroutine(beeScript.SetRestInFlowerTime(Random.Range(2f, 4f)));
    }


    //Recollecting Behavior
    public override void Update()
    {
        if (beeScript.targetFlower != null && beeScript.allowRecollecting)
        {
            //Debug.Log("Recollecting");
        }
        else //If can't recollect anymore (for example when time passed or flower deleted) get another flower target
        {
            nextState = new Idle(bee, beeScript);

            //Exit the Recollecting state
            phase = Event.Exit;
        }
    }


    //Exit the Recollecting state and reset it
    public override void Exit()
    {
        base.Exit();
    }
}
