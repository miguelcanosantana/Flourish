using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BeeAgentScript : MonoBehaviour
{
    //Components
    [SerializeField] private GameObject heightRegulator;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private GameObject[] listOfFlowers;
    private GameObject targetFlower = null;


    //Variables
    private bool isOnFlower = false;
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
        //If not already in a flower, travel to it
        if (!isOnFlower) TravelToFlower();
    }


    //Land in the flower, recollect pollen and search for another flower
    private IEnumerator LandOnFlower()
    {
        isOnFlower = true;

        DOTween.Clear();

        //Set the animation
        animator.SetBool("OnFlower", true);

        //Wait a random time before leaving
        yield return new WaitForSeconds(2);

        //Leave the flower
        SetNewDestination();

        animator.SetBool("OnFlower", false);

        isOnFlower = false;
    }


    //Travel to flower AI
    private void TravelToFlower()
    {
        //If there isn't a destination flower, set and new one and return
        if (targetFlower == null)
        {
            SetNewDestination();
            return;
        }
        else //If the flower has not disappeared, calculate remaining position time
        {
            //Do a tween between the heightRegulator Y and the target flower Y
            Debug.Log(timeBetweenTarget);
            timeBetweenTarget = agent.remainingDistance / agent.speed;
            heightRegulator.transform.DOMoveY(targetFlower.transform.position.y, timeBetweenTarget * 5);
        }

        //If the agent is in the target position,
        //it doesn't have path or it's speed is 0, land in the flower
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //Using sqrMagnitude instead of magnitude increases performance
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) StartCoroutine(LandOnFlower());
        }
    }


    //Set a new destination
    private void SetNewDestination()
    {
        //If there are no flowers, return
        if (listOfFlowers.Length == 0) return;

        //Get a random flower and set it as the target
        int randomDestination = Random.Range(0, listOfFlowers.Length);
        targetFlower = listOfFlowers[randomDestination];

        //Set the destination
        agent.SetDestination(targetFlower.transform.position);
    }

}
