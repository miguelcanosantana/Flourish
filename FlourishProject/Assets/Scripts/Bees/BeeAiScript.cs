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
    private NavMeshAgent agent;
    private BeeStateClass currentState; //The current state in the FSM (Finite State Machine)

    //Variables
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


    //Start
    void Start()
    {
        //Get the components
        agent = GetComponent<NavMeshAgent>();

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

        //Get all flowers
        List<GameObject> allFlowers = GameObject.FindGameObjectsWithTag("Flower").ToList();

        //Clear the previous flower list
        listOfFlowers.Clear();

        //Only put in list of flowers the flowers with pollen and with no bees posed
        foreach (GameObject flower in allFlowers)
        {
            FlowerDataScript tempFlowerScript = flower.GetComponent<FlowerDataScript>();

            if (tempFlowerScript.currentPollen > 0 && !tempFlowerScript.isBeePosed)
            {
                listOfFlowers.Add(flower);
            }
        }

        //Debug.Log("Updated flowers: " + listOfFlowers.Count);

        yield return new WaitForSeconds(2f);
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

}
