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
    private NavMeshAgent agent;
    private BeeStateClass currentState; //The current state in the FSM (Finite State Machine)

    //Variables
    [HideInInspector] public List<GameObject> listOfFlowers = new List<GameObject>();
    [HideInInspector] public GameObject targetFlower;
    [HideInInspector] public GameObject previousFlower;
    [HideInInspector] public bool allowRecollecting;
    [HideInInspector] public bool alreadyTweening;


    //Start
    void Start()
    {
        //Get the components
        agent = GetComponent<NavMeshAgent>();

        //Get the flowers
        listOfFlowers = GameObject.FindGameObjectsWithTag("Flower").ToList();

        //Start the FSM in idle state
        currentState = new Idle(gameObject, this);
    }


    //Update
    void Update()
    {
        currentState = currentState.Process();
    }


    //Coroutine for FSM => Set the bee to recollect in the flower for a time, all managed on the FSM
    public IEnumerator SetRestInFlowerTime(float time)
    {
        allowRecollecting = true;
        yield return new WaitForSeconds(time);
        allowRecollecting = false;
    }

}
