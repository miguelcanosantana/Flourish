using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TornadoScript : MonoBehaviour
{
    //References
    [SerializeField] private Transform suckPoint;

    private List<GameObject> objectsBeingSucked = new List<GameObject>();


    //When objects enter the tornado add them to the list
    private void OnTriggerEnter(Collider collider)
    {
        //Add the bee to the list
        if (collider.CompareTag("Bee")) objectsBeingSucked.Add(collider.gameObject);
    }


    //When objects exit the tornado remove them from the list
    private void OnTriggerExit(Collider collider)
    {
        //Remove the object from the list
        //if (collider.CompareTag("Bee")) objectsBeingSucked.Remove(collider.gameObject);
    }



    private void Update()
    {
        SuckObjects();
    }



    //Actively suck the objects
    private void SuckObjects()
    {
        foreach (GameObject suckObject in objectsBeingSucked)
        {
            //If the object is bee type suck it
            if (suckObject.CompareTag("Bee"))
            {
                GameObject beeAgentObject = suckObject.transform.parent.Find("BeeAgent").gameObject;
                BeeAiScript beeScript = suckObject.transform.parent.Find("BeeAgent").GetComponent<BeeAiScript>();

                beeScript.isBeingSucked = true;
                beeAgentObject.transform.position = Vector3.MoveTowards(beeAgentObject.transform.position, suckPoint.position, Time.deltaTime * 5f);
            }
        }
    }


    //Re-enable the bee agents, then, clear the list of objects when being disabled
    private void OnDisable()
    {
        //Re-enable the bee agents
        foreach (GameObject suckObject in objectsBeingSucked)
        {
            //Set to false the bees getting sucked
            if (suckObject != null)
            {
                BeeAiScript beeScript = suckObject.transform.parent.Find("BeeAgent").GetComponent<BeeAiScript>();
                beeScript.isBeingSucked = false;
            }
        }

        //Clear the list
        objectsBeingSucked.Clear();
    }

}
