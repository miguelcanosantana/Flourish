using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeFlowerCheckScript : MonoBehaviour
{
    //Flowers list
    private List<GameObject> allFlowers = new List<GameObject>();
    private List<GameObject> matchingFlowers = new List<GameObject>();


    //Check near flowers and only get the ones that are of the same type
    public List<GameObject> GetNearFlowersOfType(FlowerType typeToMatch)
    {
        //Reset the list
        matchingFlowers.Clear();

        //If flower matches, add it to the matching list
        foreach (GameObject flower in allFlowers)
        {
            FlowerDataScript flowerScript = flower.GetComponent<FlowerDataScript>();

            if (flowerScript.flowerType == typeToMatch) matchingFlowers.Add(flower);
        }

        Debug.Log(matchingFlowers.Count);
        return matchingFlowers;
    }


    //When a flower enters a trigger, add it to the all flowers list
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Flower")) allFlowers.Add(collider.gameObject);
    }


    //When a flower exits a trigger, remove it from the all flowers list
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Flower")) allFlowers.Remove(collider.gameObject);
    }
}
