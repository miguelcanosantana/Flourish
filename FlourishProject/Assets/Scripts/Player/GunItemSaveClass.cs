using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Custom class made for saving it on the PersistentScriptableObject
[Serializable]
public class GunItemSaveClass
{
    [Header("Enum ID")]
    public GunItemType itemType = GunItemType.None;

    [Header("Stats")]
    public bool hasBeenDiscovered = false;
    public int itemAmmount = 0;
}
