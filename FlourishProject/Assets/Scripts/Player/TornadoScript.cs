using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class TornadoScript : MonoBehaviour
{
    //References
    [SerializeField] private Transform suckPoint;

    private GameManagerScript gameManagerScript;
    private List<GameObject> objectsBeingSucked = new List<GameObject>();

    //Get the game manager
    private void Start()
    {
        gameManagerScript = FindObjectOfType<GameManagerScript>();
    }


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
        //Using .ToArray to access a copy of the list (for avoiding errors)
        foreach (GameObject suckObject in objectsBeingSucked.ToArray())
        {
            //If the object is null, remove it from the list and it's references
            if (suckObject == null)
            {
                objectsBeingSucked.Remove(suckObject);
                return;
            }

            //If the object is bee type suck it
            if (suckObject.CompareTag("Bee"))
            {
                GameObject beeAgentObject = suckObject.transform.parent.Find("BeeAgent").gameObject;
                BeeAiScript beeScript = suckObject.transform.parent.Find("BeeAgent").GetComponent<BeeAiScript>();

                beeScript.isBeingSucked = true;
                beeAgentObject.transform.LookAt(suckPoint.position);
                beeAgentObject.transform.position = Vector3.MoveTowards(beeAgentObject.transform.position, suckPoint.position, Time.deltaTime * 5f);

                float distanceIn2D = Vector2.Distance(
                    new Vector2(beeAgentObject.transform.position.x, beeAgentObject.transform.position.z),
                    new Vector2(suckPoint.transform.position.x, suckPoint.transform.position.z)
                    );

                //Remove the bee from the list and destroy it
                if (suckObject != null && distanceIn2D <= 1f)
                {
                    StartCoroutine(DestroyBee(suckObject, beeScript));
                }
            }
        }
    }


    //Destroy the bee after being sucked
    private IEnumerator DestroyBee(GameObject beeObject, BeeAiScript beeScript)
    {
        //Remove it from the list
        objectsBeingSucked.Remove(beeObject);

        //Get the bee info
        FlowerType beeFlowerMatch = beeScript.flowerTypeMatch;
        int beePollen = (int) beeScript.loadedPollen;

        //Animate it
        beeObject.transform.DOScale(0f, 0.1f).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(0.1f);

        //Add bee to the items bar
        gameManagerScript.AddBeeToBar(beeFlowerMatch, beePollen);

        //Destroy it
        if (beeObject != null)
        {
            //beeScript.DOKill();

            GameObject parentObject = beeObject.transform.parent.gameObject;
            Destroy(parentObject);
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
