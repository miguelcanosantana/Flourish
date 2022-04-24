using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SaveDataScriptable saveData;
    [SerializeField] private GameObject flowersFolder;

    [Header("Prefabs")]
    [SerializeField] private GameObject testFlowerPrefab;
    [SerializeField] private GameObject sunFlowerPrefab;
    [SerializeField] private GameObject tulipPrefab;


    private void Start()
    {
        //Load the game data
        LoadGame();
    }


    //Spawn player, flowers and bee according to loaded data
    public void LoadGame()
    {
        //Load the data from the PersistentScriptableObject
        saveData.Load();

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
    }


    //Save the player, the flowers and the bees data
    public void SaveGame()
    {
        //Get all the flowers and save them 1 by 1
        FlowerScript[] flowerScripts = FindObjectsOfType<FlowerScript>();

        foreach (FlowerScript flower in flowerScripts)
        {
            flower.SaveFlower();
        }
    }
}
