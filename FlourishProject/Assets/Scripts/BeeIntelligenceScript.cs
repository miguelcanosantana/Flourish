using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeeIntelligenceScript : MonoBehaviour
{

    //Components
    private NavMeshAgent agent;
    private GameObject[] listOfFlowers;
    private GameObject targetEntry = null;


    //Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        listOfFlowers = GameObject.FindGameObjectsWithTag("Flower");
    }


    //Update
    private void Update()
    {

        //If there isn't a destination flower, set and new one and return
        if (targetEntry == null)
        {
            SetNewDestination();
            return;
        }

        //If the agent is in the target position,
        //it doesn't have path or it's speed is 0, set a new destination
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //Using sqrMagnitude instead of magnitude increases performance
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) SetNewDestination();
        }

        //Move the agent to the target flower, if the flower has not disappeared
        if (targetEntry != null) agent.SetDestination(targetEntry.transform.position);
    }


    //Set a new destination
    private void SetNewDestination()
    {
        //If there are no flowers, return
        if (listOfFlowers.Length == 0) return;

        //Get a random flower and set it as the target
        int randomDestination = Random.Range(0, listOfFlowers.Length);

        targetEntry = listOfFlowers[randomDestination];

        //for (int i = 0; i < cityScript.buildingsEntries.Count; i++)
        //{
        //    GameObject tempEntry = cityScript.buildingsEntries[i];
        //    if (i == randomDestination) targetEntry = tempEntry;
        //}
    }


}
