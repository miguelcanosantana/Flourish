using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Custom class made for saving it on the PersistentScriptableObject
[Serializable]
public class BeeSaveClass
{
    [Header("Bee ID")]
    public string id;

    [Header("Transform")]
    public Vector3 position;
    public Quaternion rotation;

    [Header("Stats")]
    public FlowerType match;
    public int recollectionAmount;
}
