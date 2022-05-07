using DG.Tweening;
using System;
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


public class FlowerScript : MonoBehaviour
{
    [Header("Flower ID")]
    public string id = "None";

    [Header("Stats")]
    public FlowerType flowerType = FlowerType.None;
    public int maxPollen;
    public int regeneratePollenRate;

    [Header("References")]
    [SerializeField] private GameObject flowerParentObject;
    [SerializeField] private GameObject flowerPetals;
    [SerializeField] private SaveDataScriptable saveData;
    [SerializeField] private GameObject barObject;
    [SerializeField] private Image pollenBarBackground;
    [SerializeField] private Image pollenBarLevel;

    private Camera playerCamera;

    //Variables
    [HideInInspector] public bool hasBeenLoaded = false;
    [HideInInspector] public int age = 0; //1 second = 1 day
    [HideInInspector] public int currentPollen = 0;
    [HideInInspector] public bool isBeePosed = false;

    private bool canRegeneratePollen = true;
    private bool canUpdateAge = true;


    //Start
    private void Start()
    {
        //Get the player camera
        playerCamera = Camera.main;

        UpdatePollenUI();

        //Animate flower with easing if hasn't been loaded
        if (!hasBeenLoaded)
        {
            flowerParentObject.transform.localScale = Vector3.zero;
            flowerPetals.transform.localScale = Vector3.zero;
            flowerParentObject.transform.DOScale(1f, 0.55f).SetEase(Ease.OutExpo);
            flowerPetals.transform.DOScale(1f, 3f).SetEase(Ease.OutExpo);
        }
    }


    //Update
    private void Update()
    {
        //If no bees are posed and can regenerate the pollen, do 1 unit and wait some time before doing it again
        if (canRegeneratePollen && currentPollen < maxPollen && !isBeePosed) StartCoroutine(RegeneratePollen());

        UpdateUIPosition();

        //Update the age of the flower (Every 1 second)
        if (canUpdateAge) StartCoroutine(UpdateAge());
    }


    //Coroutine => Update Flower Age
    private IEnumerator UpdateAge()
    {
        canUpdateAge = false;

        yield return new WaitForSeconds(1f);
        age++;

        canUpdateAge = true;
    }


    //Save Flower when called from GameManager
    public void SaveFlower()
    {
        //If flower id is None, Generate a guid and convert it to string
        if (id == "None") id = Guid.NewGuid().ToString();

        saveData.SaveFlower(
            id, 
            flowerParentObject.transform.position, 
            flowerParentObject.transform.rotation,
            flowerType,
            age,
            currentPollen);
    }


    //Update the position of the UI
    private void UpdateUIPosition()
    {
        barObject.transform.LookAt(playerCamera.transform.position);
        barObject.transform.Rotate(0, 180, 0);
        barObject.transform.rotation = Quaternion.Euler(0, barObject.transform.rotation.eulerAngles.y, 0);
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
        //Fill the bar depending on the pollen level
        float percentage = ((float) currentPollen / (float) maxPollen);
        pollenBarLevel.fillAmount = percentage;
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
        //Else if there is only a bit of pollen, take that little amount
        else if (currentPollen < pollenToTake && currentPollen > 0)
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
