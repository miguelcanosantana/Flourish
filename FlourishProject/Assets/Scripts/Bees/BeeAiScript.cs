using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class BeeAiScript : MonoBehaviour
{
    //References
    public GameObject beeContainerObject;
    public Animator animator;
    public GameObject rangeChecker;
    private RangeFlowerCheckScript rangeCheckScript;
    private NavMeshAgent agent;
    private BeeStateClass currentState; //The current state in the FSM (Finite State Machine)

    //Variables
    public FlowerType flowerTypeMatch;
    public int recollectionAmount;
    [HideInInspector] public List<GameObject> listOfFlowers = new List<GameObject>();
    [HideInInspector] public GameObject targetFlower;
    [HideInInspector] public FlowerDataScript targetFlowerScript;
    [HideInInspector] public GameObject previousFlower;
    [HideInInspector] public bool allowRecollecting;
    [HideInInspector] public bool alreadyTweening;
    [HideInInspector] public bool canUpdateFlowers = true;
    [HideInInspector] public float loadedPollen = 0f;
    [HideInInspector] public Sequence flySequence;
    [HideInInspector] public bool canUpdatePosedTimer = true;
    [HideInInspector] public int timerSinceLastPosed = 0;
    [HideInInspector] public float maxTimeToPose = 0f;


    //Start
    void Start()
    {
        //Get the components
        agent = GetComponent<NavMeshAgent>();
        rangeCheckScript = rangeChecker.GetComponent<RangeFlowerCheckScript>();

        //Start the FSM in idle state
        currentState = new Idle(gameObject, this);
    }


    //Update
    void Update()
    {
        currentState = currentState.Process();
    }


    //Coroutine for FSM => Update the list of flowers the bee can travel to (Using a timer to increase performance)
    public IEnumerator UpdateFlowersList()
    {
        canUpdateFlowers = false;

        //Get the flowers from the range checker sphere trigger
        listOfFlowers = rangeCheckScript.GetNearFlowersOfType(flowerTypeMatch);

        yield return new WaitForSeconds(0.5f);
        canUpdateFlowers = true;
    }


    //Coroutine for FSM => Wait a time and recollect some pollen from the flower 
    public IEnumerator RecollectPollen(float time)
    {
        allowRecollecting = true;

        yield return new WaitForSeconds(time);
        FlowerDataScript flowerScript = targetFlower.GetComponent<FlowerDataScript>();
        loadedPollen += flowerScript.TryTakePollen(recollectionAmount);

        allowRecollecting = false;
    }


    //Coroutine for FSM => Update the timer since last posed 
    public IEnumerator UpdateLastPosedTimer()
    {
        canUpdatePosedTimer = false;

        yield return new WaitForSeconds(1f);

        timerSinceLastPosed ++;

        Debug.Log(timerSinceLastPosed);

        canUpdatePosedTimer = true;
    }

}
