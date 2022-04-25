using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [Header("Player Stats")]
    public int playerPollen = 0;
    public int playerHappiness = 0;
    public List<GunItemSaveClass> playerGunItems = new List<GunItemSaveClass>();

    [Header("References")]
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
        //LoadGame();
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

        //Transform
        playerObject.transform.position = saveData.playerPosition;
        playerObject.transform.rotation = saveData.playerRotation;

        //Stats
        playerPollen = saveData.playerPollen;
        playerHappiness = saveData.playerHappiness;

        //Gun items
        foreach (GunItemSaveClass item in playerGunItems)
        {

        }

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
    }


    //Save the player, the flowers and the bees data
    public void SaveEntireGame()
    {
        //Get the player and save it's transform and properties
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerControlScript playerControlScript = playerObject.GetComponent<PlayerControlScript>();

        playerControlScript.SavePlayerTransform();

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
    }
}
