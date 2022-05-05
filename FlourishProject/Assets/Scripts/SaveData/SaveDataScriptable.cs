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

    [Header("Player's Stats")]
    public int playerPollen = 0;
    public int playerHappiness = 0;
    
    [Header("Lists")]
    public List<GunItemSaveClass> playerGunItems = new List<GunItemSaveClass>();
    public List<FlowerSaveClass> flowerSaves = new List<FlowerSaveClass>();
    public List<BeeSaveClass> beeSaves = new List<BeeSaveClass>();



    //Copy all of the content in one save to the main save, and store it
    public void CopyFromXToSave(SaveDataScriptable xSave, SaveDataScriptable mainSave)
    {
        //xSave.Load();
        //mainSave.Load();

        mainSave.playerPosition = xSave.playerPosition;
        mainSave.playerRotation = xSave.playerRotation;
        mainSave.playerPollen = xSave.playerPollen;
        mainSave.playerHappiness = xSave.playerHappiness;
        mainSave.playerGunItems = xSave.playerGunItems;
        mainSave.flowerSaves = xSave.flowerSaves;
        mainSave.beeSaves = xSave.beeSaves;

        Save();
    }


    //Save player Transform
    public void SavePlayerTransform(Vector3 position, Quaternion rotation)
    {
        playerPosition = position;
        playerRotation = rotation;

        Save();
    }


    //Save player stats
    public void SavePlayerStats(int pollen, int happiness)
    {

        //Stats
        playerPollen = pollen;
        playerHappiness = happiness;

        Save();
    }


    //Save player gun items
    public void SaveGunItem(GunItemType type, bool hasAmount, int ammount)
    {
        //Check if the type of that item already exists
        foreach (GunItemSaveClass item in playerGunItems)
        {
            //If there is already a gun item of that type, add or subtract
            if (item.itemType == type)
            {
                //Update info
                item.hasAmount = hasAmount;

                //Save item amount
                item.itemAmount = Mathf.Clamp(ammount, 0, 999);

                Save();
                return;
            }
        }

        //If the item is not present in the items types list, add it
        GunItemSaveClass gunItemSave = new GunItemSaveClass();

        //ID (Using enum as ID to avoid duplicated item types)
        gunItemSave.itemType = type;

        //Stats
        gunItemSave.hasAmount = hasAmount;
        gunItemSave.itemAmount = ammount;

        //Add to the list
        playerGunItems.Add(gunItemSave);

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