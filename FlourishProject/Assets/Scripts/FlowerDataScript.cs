using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FlowerType
{
    None,
    Sunflower,
    Tulip,
}


public class FlowerDataScript : MonoBehaviour
{
    [Header("Stats")]
    public FlowerType flowerType = FlowerType.None;
    [Range(20, 50)] public int maxPollen;

    //Variables
    public float age = 0; //1 second = 1 day
    public int currentPollen = 0;
    [HideInInspector] public bool isAvailable = false;
    [HideInInspector] public bool isBeePosed = false;

    private float timeWhenCreated = 0f;
    private float timeActive = 0f;


    //Start
    private void Start()
    {
        //Save the time when it was created
        timeWhenCreated = Time.timeSinceLevelLoad;
    }


    //Check the time since the level has loaded
    private void UpdateActiveTime()
    {
        timeActive = Time.timeSinceLevelLoad - timeWhenCreated;
    }


    //Update
    private void Update()
    {
        UpdateActiveTime();
        //Debug.Log(timeActive);
    }


}
