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
    public enum State { Idle, Traveling, Recollecting, LostDestroy};

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

        //If not already tweening and there is no path pending, regulate height
        if (!beeScript.alreadyTweening && !agent.pathPending && agent.remainingDistance > 0)
        {
            beeScript.alreadyTweening = true;

            float time = GetAgentDistance(agent) / agent.speed;

            //Set the max time to pose
            beeScript.maxTimeToPose = time + 2f;

            //Create a sequence and add the movement to it
            beeScript.flySequence = DOTween.Sequence();
            beeScript.flySequence.Append(beeScript.beeContainerObject.transform.DOMoveY(beeScript.targetFlower.transform.position.y, time));
            beeScript.flySequence.Play();
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

        //Kill previously tweens
        beeScript.flySequence.Kill();
        
        //Reset the last time since posed
        beeScript.timerSinceLastPosed = 0;
    }


    //Idle's behavior
    public override void Update()
    {
        //Update flowers list on the bee if it can (Increases performance waiting a time each attempt)
        if (beeScript.canUpdateFlowers) beeScript.StartCoroutine(beeScript.UpdateFlowersList());

        //Enter the Traveling state immediately if the list of flowers has at least 2
        if (beeScript.listOfFlowers.Count >= 2)
        {
            //Set as not lost and set it's time to 0
            beeScript.isLost = false;
            beeScript.timeLost = 0;
            beeScript.SetExpression(BeeExpression.Smile);

            //If the bee had a target flower if coming from a State, set it as the previous flower
            if (beeScript.targetFlower != null)
            {
                //Set as previous
                beeScript.previousFlower = beeScript.targetFlower;
            }
                
            //Select a flower as a target, if it's the same flower as the previous one, repeat
            do
            {
                int randomDestination = Random.Range(0, beeScript.listOfFlowers.Count);
                beeScript.targetFlower = beeScript.listOfFlowers[randomDestination];
                beeScript.targetFlowerScript = beeScript.targetFlower.GetComponent<FlowerScript>();

            } while (beeScript.targetFlower == beeScript.previousFlower);

            nextState = new Traveling(bee, beeScript);

            //Exit the Idle state
            phase = Event.Exit;
        }
        //If the bee has no flowers near, get lost
        else
        {
            beeScript.isLost = true;
            beeScript.SetExpression(BeeExpression.Lost);
        }

        //Increase lost time
        if (beeScript.isLost)
        {
            beeScript.timeLost += Time.deltaTime;

            if (beeScript.timeLost > 5f)
            {
                //Exit this state and enter LostDestroy state
                nextState = new LostDestroy(bee, beeScript);

                //Exit the Idle state
                phase = Event.Exit;
            }
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

        //Set the destination if the flower exists and set as targeted
        if (beeScript.targetFlower)
        {
            agent.SetDestination(beeScript.targetFlower.transform.position);
        }
    }


    //Traveling Behavior
    public override void Update()
    {
        //If the bee is being sucked, exit to idle state
        if (beeScript.isBeingSucked)
        {
            agent.angularSpeed = 0f;
            agent.speed = 0f;
            agent.acceleration = 0f;

            nextState = new Idle(bee, beeScript);

            //Exit the Traveling state
            phase = Event.Exit;
        }
        //Restore the properties
        else
        {
            agent.angularSpeed = beeScript.angularSpeedBackup;
            agent.speed = beeScript.speedBackup;
            agent.acceleration = beeScript.accelerationBackup;
        }

        //Regulate the position and the height
        RegulatePositionHeight();

        //Update the time since last posed in 1 second interval
        if (beeScript.canUpdatePosedTimer) beeScript.StartCoroutine(beeScript.UpdateLastPosedTimer());

        //If the bee is stuck and hasn't reached the flower, change target
        if (beeScript.timerSinceLastPosed > beeScript.maxTimeToPose)
        {
            nextState = new Idle(bee, beeScript);

            //Exit the Traveling state
            phase = Event.Exit;
        }

        //If the target flower is null or the bee is posed, choose another target
        if (beeScript.targetFlower == null || beeScript.targetFlowerScript.isBeePosed)
        {
            nextState = new Idle(bee, beeScript);

            //Exit the Traveling state
            phase = Event.Exit;
        }

        //If the agent is in the target position, land in the flower
        if (agent.remainingDistance <= 0.1f && !agent.pathPending)
        {
            nextState = new Recollecting(bee, beeScript);

            //Exit the Traveling state
            phase = Event.Exit;
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

        //Recollect some pollen from the flower
        if (beeScript.targetFlower != null)
        {
            //Reset the timer since last posed
            beeScript.timerSinceLastPosed = 0;

            //Set in the flower that bee is posed
            beeScript.targetFlower.GetComponent<FlowerScript>().isBeePosed = true;

            float recollectWait = Random.Range(2f, 4f);
            beeScript.StartCoroutine(beeScript.RecollectPollen(recollectWait));
        }

        //Reset the last time since posed
        beeScript.timerSinceLastPosed = 0;
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
            //Set in the flower that bee is not posed anymore
            beeScript.targetFlower.GetComponent<FlowerScript>().isBeePosed = false;

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


//LostDestroy state
public class LostDestroy : BeeStateClass
{
    //LostDestroy's constructor (Take the bee from the base class)
    public LostDestroy(GameObject beeObject, BeeAiScript script) : base(beeObject, script)
    {
        stateName = State.LostDestroy;
    }


    //Enter the LostDestroy
    public override void Enter()
    {
        base.Enter();
        beeScript.StartCoroutine(beeScript.DestroyBee());
    }


    //LostDestroy Behavior
    public override void Update()
    {
    }


    //Exit the LostDestroy state and destroy this object
    public override void Exit()
    {
        base.Exit();
    }
}
