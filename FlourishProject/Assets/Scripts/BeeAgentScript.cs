using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BeeAgentScript : MonoBehaviour
{
    //Components
    [SerializeField] private GameObject heightRegulator;

    private NavMeshAgent agent;
    private GameObject[] listOfFlowers;
    private GameObject targetFlower = null;

    //Variables
    private float timeBetweenTarget = 0f;


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
        if (targetFlower == null)
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
        if (targetFlower != null) agent.SetDestination(targetFlower.transform.position);

        //Vector3 positionToMove = new Vector3(transform.position.x, targetFlower.transform.position.y, transform.position.z);
        //heightRegulator.transform.DOMove(positionToMove, 4);

        //Do a tween between the heightRegulator Y and the target flower Y
        timeBetweenTarget = agent.remainingDistance / agent.speed;
        heightRegulator.transform.DOMoveY(targetFlower.transform.position.y, timeBetweenTarget * 5);
        Debug.Log(timeBetweenTarget);
    }


    //Set a new destination
    private void SetNewDestination()
    {
        //If there are no flowers, return
        if (listOfFlowers.Length == 0) return;

        //Get a random flower and set it as the target
        int randomDestination = Random.Range(0, listOfFlowers.Length);
        targetFlower = listOfFlowers[randomDestination];
    }

}
