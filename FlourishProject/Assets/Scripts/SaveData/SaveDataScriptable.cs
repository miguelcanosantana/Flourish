using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "Save", menuName = "New Save")]
public class SaveDataScriptable : PersistentScriptableObject
{
    [Header("Player's Transform")]
    public Vector3 playerPosition = Vector3.zero;
    public Quaternion playerRotation = Quaternion.identity;


    [Header("Lists")]
    public List<FlowerSaveClass> flowerSaves = new List<FlowerSaveClass>();
    public List<BeeSaveClass> beeSaves = new List<BeeSaveClass>();


    //Save data methods with override
    public void SavePlayerTransform(Vector3 position, Quaternion rotation)
    {
        playerPosition = position;
        playerRotation = rotation;
        Save();
    }


    //Save a new flower's data (and return it's guid), or if it already exists, update it
    public void SaveFlower(string id, Vector3 position, Quaternion rotation, FlowerType type, int age, int pollen)
    {
        
        //Check if flower exists
        foreach (FlowerSaveClass flower in flowerSaves)
        {
            //If the flower already exists in the data, update the flower
            if (flower.id == id)
            {
                //Update only stats
                flower.age = age;
                flower.pollen = pollen;

                Save();
                return;
            }
        }

        //If the flower does not exist in the data, create a new one
        FlowerSaveClass flowerSave = new FlowerSaveClass();

        //ID
        flowerSave.id = id;

        //Transform
        flowerSave.position = position;
        flowerSave.rotation = rotation;

        //Stats
        flowerSave.type = type;
        flowerSave.age = age;
        flowerSave.pollen = pollen;

        //Add to the list
        flowerSaves.Add(flowerSave);

        Save();
    }


    //Save a new bee's data (and return it's guid), or if it already exists, update it
    public void SaveBee(string id, Vector3 position, Quaternion rotation, FlowerType match, int recollection)
    {

        //Check if bee exists
        foreach (BeeSaveClass bee in beeSaves)
        {
            //If the bee already exists in the data, update the bee
            if (bee.id == id)
            {
                //Update only stats
                bee.position = position;
                bee.rotation = rotation;
                bee.recollectionAmount = recollection;

                Save();
                return;
            }
        }

        //If the bee does not exist in the data, create a new one
        BeeSaveClass beeSave = new BeeSaveClass();

        //ID
        beeSave.id = id;

        //Transform
        beeSave.position = position;
        beeSave.rotation = rotation;

        //Stats
        beeSave.match = match;
        beeSave.recollectionAmount = recollection;

        //Add to the list
        beeSaves.Add(beeSave);

        Save();
    }

}