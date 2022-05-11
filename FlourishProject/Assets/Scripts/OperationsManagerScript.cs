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

    //Stick trees to the floor
    [EButton("Floor stick trees")]
    public void FloorStick()
    {

        //Get the terrain and it's Y position
        if (terrain == null) FindObjectOfType<Terrain>();

        int terrainYPos = (int) terrain.transform.position.y;

        GameObject[] allTrees = GameObject.FindGameObjectsWithTag("Tree");

        foreach (GameObject tree in allTrees)
        {

            for (int i = terrainYPos + 200; i > terrainYPos - 200; i--)
            {
                tree.transform.position = new Vector3(tree.transform.position.x, i, tree.transform.position.z);

                if (Physics.Raycast(tree.transform.position, -Vector3.up, 0.1f)) break;
            }


            //float randomAngle = Random.Range(0, 360);
            //tree.transform.Rotate(Vector3.up, randomAngle);
        }
    }
}
