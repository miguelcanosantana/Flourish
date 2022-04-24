using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//Custom class made for saving it on the PersistentScriptableObject
[Serializable]
public class FlowerSaveClass
{
    [Header("Flower ID")]
    public string id;

    [Header("Transform")]
    public Vector3 position;
    public Quaternion rotation;

    [Header("Stats")]
    public FlowerType type;
    public int age;
    public int pollen;
}
