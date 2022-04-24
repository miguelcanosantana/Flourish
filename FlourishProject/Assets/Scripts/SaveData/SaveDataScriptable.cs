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


    [Header("Flowers")]
    public List<FlowerSaveClass> flowerSaves = new List<FlowerSaveClass>();


    //Save data methods with override
    public void SavePlayerTransform(Vector3 position, Quaternion rotation)
    {
        playerPosition = position;
        playerRotation = rotation;
        Save();
    }


    //Save a new flower's data (and return it's guid), or if it already exists, update it
    public string SaveFlower(string id, Vector3 position, Quaternion rotation, FlowerType type, int age, int pollen)
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

                //Don't return a GUID
                return "None";
            }
        }

        //If the flower does not exist in the data, create a new one
        FlowerSaveClass flowerSave = new FlowerSaveClass();

        //Generate a guid and convert it to string 
        string guid = new System.Guid().ToString();

        //ID
        flowerSave.id = guid;

        //Transform
        flowerSave.position = position;
        flowerSave.rotation = rotation;

        //Stats
        flowerSave.type = type;
        flowerSave.age = age;
        flowerSave.pollen = pollen;

        //Add to the list
        flowerSaves.Add(flowerSave);

        //Return the flower GUID
        return guid;
    }

}