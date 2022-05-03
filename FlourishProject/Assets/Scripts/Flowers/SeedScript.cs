using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedScript : MonoBehaviour
{
    [Header("Stats")]
    public FlowerType seedFlowerType;

    [Header("References")]
    public GameObject testFlowerPrefab;
    public GameObject sunFlowerPrefab;
    public GameObject tulipPrefab;

    private GameObject flowersFolder;
    private Transform playerTrasform;


    private void Start()
    {
        //Get the objects
        flowersFolder = GameObject.FindGameObjectWithTag("FlowersFolder");
        playerTrasform = GameObject.FindGameObjectWithTag("Player").transform;
    }


    //Check object distance from the player, if it's too far (Fell of the world, disable it)
    private void Update()
    {
        if (Vector3.Distance(transform.position, playerTrasform.position) > 30) gameObject.SetActive(false);
    }


    //Plant a flower of the seed type
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GameObject flowerToInstantiate = testFlowerPrefab;

            switch (seedFlowerType)
            {
                case FlowerType.Sunflower:
                    flowerToInstantiate = sunFlowerPrefab;
                    break;
                case FlowerType.Tulip:
                    flowerToInstantiate = tulipPrefab;
                    break;
            }

            GameObject tempFlower = Instantiate(flowerToInstantiate, transform.position, Quaternion.identity);
            tempFlower.transform.parent = flowersFolder.transform;

            //Deactivate
            gameObject.SetActive(false);
        }
    }
}
