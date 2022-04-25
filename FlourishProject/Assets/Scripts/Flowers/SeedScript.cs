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


    private void Start()
    {
        //Get the flowers folder
        flowersFolder = GameObject.FindGameObjectWithTag("FlowersFolder");
    }


    //Plant a flower of the seed type
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Ground"))
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
        }
    }
}
