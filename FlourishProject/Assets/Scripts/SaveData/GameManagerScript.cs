using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GameManagerScript : MonoBehaviour
{
    [Header("Stats")]
    public int maxAmountPerItem = 60;

    [Header("Player Stats")]
    public int playerPollen = 0;
    public int playerHappiness = 0;
    public List<GunItemInfoClass> playerGunItems = new List<GunItemInfoClass>();

    [Header("References")]
    public GameObject itemsBarContent;
    public GameObject selectedItemFrame;
    [SerializeField] private SaveDataScriptable saveData;
    [SerializeField] private GameObject flowersFolder;
    [SerializeField] private GameObject beesFolder;

    [Header("Item prefabs")]
    [SerializeField] private GameObject barItemPrefab;

    [Header("Flowers Prefabs")]
    [SerializeField] private GameObject testFlowerPrefab;
    [SerializeField] private GameObject sunFlowerPrefab;
    [SerializeField] private GameObject tulipPrefab;

    [Header("Bees Prefabs")]
    [SerializeField] private GameObject regularBeePrefab;


    private void Start()
    {
        //Load the game data
        LoadGame();

        //Create empty item if the list is 0 (First time playing)
        if (playerGunItems.Count == 0)
        {
            GunItemInfoClass emptyItem = new GunItemInfoClass();
            playerGunItems.Add(emptyItem);

            saveData.SaveGunItem(emptyItem.itemType, emptyItem.hasAmount, emptyItem.itemAmount);
            RefreshBarUI();
        }
    }


    //Refresh Items bar UI
    public void RefreshBarUI()
    {
        //Delete all previous items
        foreach (Transform child in itemsBarContent.transform)
        {
            Destroy(child.gameObject);
        }

        //Create all bar items again
        foreach (GunItemInfoClass item in playerGunItems)
        {
            GameObject tempItem = Instantiate(barItemPrefab, itemsBarContent.transform, false);

            //Get the text mesh pro components
            TextMeshProUGUI quantityText = tempItem.transform.Find("QuantityText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI maxText = tempItem.transform.Find("MaxText").GetComponent<TextMeshProUGUI>();

            //Match the amount text with the class data
            if (item.hasAmount) quantityText.text = item.itemAmount.ToString("00");
            else quantityText.text = "";

            //Set the max amount with the class data
            if (item.hasAmount) maxText.text = "/" + maxAmountPerItem.ToString("00");
            else maxText.text = "";
        }
    }


    //Load and save data (ONLY FOR DEBUGGING)
    void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 100, 90), "Data Manager");

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(20, 40, 80, 20), "Load"))
        {
            LoadGame();
        }

        // Make the second button.
        if (GUI.Button(new Rect(20, 70, 80, 20), "Save"))
        {
            SaveEntireGame();
        }
    }


    //Spawn player, flowers and bee according to loaded data
    public void LoadGame()
    {
        //Load the data from the PersistentScriptableObject
        saveData.Load();

        //Load player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        //Transform (Disable and re-enable character controller)
        playerObject.GetComponent<CharacterController>().enabled = false;

        playerObject.transform.localPosition = saveData.playerPosition;
        playerObject.transform.rotation = saveData.playerRotation;

        playerObject.GetComponent<CharacterController>().enabled = true;

        //Stats
        playerPollen = saveData.playerPollen;
        playerHappiness = saveData.playerHappiness;

        //Load Gun items
        foreach (GunItemSaveClass itemSave in saveData.playerGunItems)
        {
            GunItemInfoClass tempItem = new GunItemInfoClass();

            tempItem.itemType = itemSave.itemType;
            tempItem.hasAmount = itemSave.hasAmount;
            tempItem.itemAmount = itemSave.itemAmount;

            playerGunItems.Add(tempItem);
        }

        RefreshBarUI();

        //Load flowers
        foreach (FlowerSaveClass flower in saveData.flowerSaves)
        {
            GameObject tempFlowerObject = testFlowerPrefab;

            //Instantiate a flower prefab depending on the Flower type
            switch (flower.type)
            {
                case FlowerType.None:
                    Debug.LogError("Retrieved flower must be from a type");
                    break;

                case FlowerType.Sunflower:
                    tempFlowerObject = Instantiate(sunFlowerPrefab);
                    break;

                case FlowerType.Tulip:
                    tempFlowerObject = Instantiate(tulipPrefab);
                    break;
            }

            //Get the flower script
            FlowerScript tempFlowerScript = tempFlowerObject.GetComponentInChildren<FlowerScript>();

            //Instantiate a flower with the following properties

            //ID
            tempFlowerScript.id = flower.id;

            //Transform
            tempFlowerObject.transform.position = flower.position;
            tempFlowerObject.transform.rotation = flower.rotation;
            tempFlowerObject.transform.parent = flowersFolder.transform;

            //Stats
            tempFlowerScript.age = flower.age;
            tempFlowerScript.currentPollen = flower.pollen;

            //Set as loaded
            tempFlowerScript.hasBeenLoaded = true;
        }

        //Load bees
        foreach (BeeSaveClass bee in saveData.beeSaves)
        {
            GameObject tempBeeObject = testFlowerPrefab;

            //Instantiate a bee prefab depending on the Flower Match type
            switch (bee.match)
            {
                case FlowerType.None:
                    Debug.LogError("Retrieved bee must have a flower match type");
                    break;

                case FlowerType.Sunflower:
                    tempBeeObject = Instantiate(regularBeePrefab);
                    break;

                case FlowerType.Tulip:
                    break;
            }

            //Get the bee script
            BeeAiScript tempBeeScript = tempBeeObject.GetComponentInChildren<BeeAiScript>();

            //Instantiate a bee with the following properties

            //ID
            tempBeeScript.id = bee.id;

            //Transform
            tempBeeObject.transform.position = bee.position;
            tempBeeObject.transform.rotation = bee.rotation;
            tempBeeObject.transform.parent = beesFolder.transform;

            //Stats
            tempBeeScript.recollectionAmount = bee.recollectionAmount;
        }

        //Destroy the loading screen
        LoadScreenScript loadScreenScript = FindObjectOfType<LoadScreenScript>();
        if (loadScreenScript != null) StartCoroutine(loadScreenScript.DestroyCanvas());
    }


    //Save the player, the flowers and the bees data
    public void SaveEntireGame()
    {
        //Get the player and save it's transform and properties
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerControlScript playerControlScript = playerObject.GetComponent<PlayerControlScript>();

        playerControlScript.SavePlayerTransform();

        //Get all the gun items and save them 1 by 1
        foreach (GunItemInfoClass item in playerGunItems)
        {
            saveData.SaveGunItem(item.itemType, item.hasAmount, item.itemAmount);
        }

        //Get all the flowers and save them 1 by 1
        FlowerScript[] flowerScripts = FindObjectsOfType<FlowerScript>();

        foreach (FlowerScript flower in flowerScripts)
        {
            flower.SaveFlower();
        }

        //Get all the bees and save them 1 by 1
        BeeAiScript[] beeAiScripts = FindObjectsOfType<BeeAiScript>();

        foreach (BeeAiScript bee in beeAiScripts)
        {
            bee.SaveBee();
        }

        //Set as there is a save
        PlayerPrefs.SetInt("anySavesPresent", 1);
    }
}
