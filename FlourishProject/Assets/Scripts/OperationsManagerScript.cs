using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[ExecuteInEditMode]
public class OperationsManagerScript : MonoBehaviour
{
    //Variables
    private Terrain terrain;


    //Rotates the trees randomly
    [EButton("Random trees rotate")]
    public void RandomRotation()
    {
        GameObject[] allTrees = GameObject.FindGameObjectsWithTag("Tree");

        foreach (GameObject tree in allTrees)
        {
            float randomAngle = Random.Range(0, 360);
            tree.transform.Rotate(Vector3.up, randomAngle);
        }
    }


    //Sizes the trees randomly
    [EButton("Random trees size")]
    public void RandomSize()
    {
        GameObject[] allTrees = GameObject.FindGameObjectsWithTag("Tree");

        float variation = 0.1f;

        foreach (GameObject tree in allTrees)
        {
            float scaleX = Random.Range(transform.localScale.x - variation, transform.localScale.x + variation);
            float scaleY = Random.Range(transform.localScale.y - variation, transform.localScale.y + variation);
            float scaleZ = Random.Range(transform.localScale.z - variation, transform.localScale.z + variation);

            tree.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }
    }
}
