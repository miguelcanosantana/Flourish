using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int maxPollen;
    public int regeneratePollenRate;

    [Header("References")]
    [SerializeField] private Image pollenBarBackground;
    [SerializeField] private Image pollenBarLevel;

    //Variables
    [HideInInspector] public float age = 0; //1 second = 1 day
    [HideInInspector] public int currentPollen = 0;
    [HideInInspector] public bool isBeePosed = false;

    private float timeWhenCreated = 0f;
    private float timeActive = 0f;
    private bool canRegeneratePollen = true;


    //Start
    private void Start()
    {
        //Save the time when it was created
        timeWhenCreated = Time.timeSinceLevelLoad;

        UpdatePollenUI();
    }


    //Check the time since the level has loaded
    private void UpdateActiveTime()
    {
        timeActive = Time.timeSinceLevelLoad - timeWhenCreated;
    }


    //Update
    private void Update()
    {
        //If can regenerate the pollen, do 1 unit and wait some time before doing it again
        if (canRegeneratePollen && currentPollen < maxPollen) StartCoroutine(RegeneratePollen());

        UpdateActiveTime();
        //Debug.Log(timeActive);
    }


    //Coroutine => Regenerate pollen
    private IEnumerator RegeneratePollen()
    {
        canRegeneratePollen = false;
        yield return new WaitForSeconds(regeneratePollenRate);

        currentPollen++;
        UpdatePollenUI();
        canRegeneratePollen = true;
    }


    //Update the elements in the pollen UI 
    private void UpdatePollenUI()
    {
        float percentage = ((float) currentPollen / (float) maxPollen);
        pollenBarLevel.fillAmount = percentage;
        Debug.Log(percentage);
    }


    //Try to take the flower pollen
    public float TryTakePollen(int pollenToTake)
    {
        //If the flowers has the pollen the bee can take, take it
        if (currentPollen >= pollenToTake)
        {
            currentPollen -= pollenToTake;
            return pollenToTake;
        }
        //Else if there is only a bit of pollen, take that little amount and mark the flower as not available
        else if (currentPollen > 0)
        {
            int pollenBackup = currentPollen;
            currentPollen = 0;
            return pollenBackup;
        }
        //Else if there is no pollen, return 0f
        else
        {
            return 0f;
        }
    }


}