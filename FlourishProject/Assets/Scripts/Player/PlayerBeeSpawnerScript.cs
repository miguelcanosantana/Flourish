using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBeeSpawnerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject backPackObject;

    //Variables
    private List<GameObject> nearFlowers = new List<GameObject>();
    private List<GameObject> nearBees = new List<GameObject>();
    private bool canCheck = true;


    //Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
    }


    //Check the flowers and the bees around, spawn necessary bees
    private IEnumerator CheckFlowersAndBeesAround()
    {
        canCheck = false;

        Debug.Log("Check: " + Time.time);

        yield return new WaitForSeconds(1f);

        canCheck = true;
    }


    //On trigger enter
    private void OnTriggerEnter(Collider collider)
    {
        //Add the flower to the list if it's near
        if (collider.gameObject.tag == "Flower") nearFlowers.Add(collider.gameObject);

        //Add the bee to the list if it's near
        if (collider.gameObject.tag == "Bee") nearBees.Add(collider.gameObject);

        //Check near bees and flowers
        if (canCheck) StartCoroutine(CheckFlowersAndBeesAround());
    }


    //On trigger exit
    private void OnTriggerExit(Collider collider)
    {
        //Remove the flower from the list if it's far
        if (collider.gameObject.tag == "Flower") nearFlowers.Remove(collider.gameObject);

        //Remove the bee from the list if it's far
        if (collider.gameObject.tag == "Bee") nearBees.Remove(collider.gameObject);
    }
}
