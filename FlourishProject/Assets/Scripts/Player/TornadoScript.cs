using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoScript : MonoBehaviour
{
    //References
    private List<GameObject> objectsBeingSucked = new List<GameObject>();


    //When objects enter the tornado add them to the list
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("TTTEEEST");

        //If the object is not already on the list and it's a bee, add it
        if (!objectsBeingSucked.Contains(collider.gameObject) && collider.gameObject.CompareTag("Bee"))
        {
            objectsBeingSucked.Add(collider.gameObject);
        }
    }


    //When objects exit the tornado remove them from the list
    private void OnTriggerExit(Collider collider)
    {
        //If the object is in the list and it's a bee, remove it
        if (objectsBeingSucked.Contains(collider.gameObject) && collider.gameObject.CompareTag("Bee"))
        {
            objectsBeingSucked.Remove(collider.gameObject);
        }
    }

}
