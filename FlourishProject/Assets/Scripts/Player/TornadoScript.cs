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
        //If the object is not already on the list and it's a bee (Suck Bee layer), add it
        if (!objectsBeingSucked.Contains(collider.gameObject) && collider.gameObject.layer == 11) 
        {
            objectsBeingSucked.Add(collider.gameObject);
        }
    }


    //When objects exit the tornado remove them from the list
    private void OnTriggerExit(Collider collider)
    {
        //If the object is in the list and it's a bee (Suck Bee layer), remove it
        if (objectsBeingSucked.Contains(collider.gameObject) && collider.gameObject.layer == 11)
        {
            objectsBeingSucked.Remove(collider.gameObject);
        }
    }

}
