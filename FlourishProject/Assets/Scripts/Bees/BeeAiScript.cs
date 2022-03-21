using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class BeeAiScript : MonoBehaviour
{
    //References
    private NavMeshAgent agent;
    private Animator animator;
    private BeeStateClass currentState; //The current state in the FSM (Finite State Machine)
    
    //Variables
    [HideInInspector] public List<GameObject> listOfFlowers = new List<GameObject>();
    public GameObject targetFlower;


    //Start
    void Start()
    {
        //Get the components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

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

}
