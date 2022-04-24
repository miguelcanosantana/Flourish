using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


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


    //Save a new flower

}