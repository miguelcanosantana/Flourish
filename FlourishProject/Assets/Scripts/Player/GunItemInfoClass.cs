using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Copy class made for having 2 different classes, this one to use, GunItemsSaveClass to save the info
[Serializable] //Added this so it appears on the inspector
public class GunItemInfoClass
{
    [Header("Enum ID")]
    public GunItemType itemType = GunItemType.None;

    [Header("Stats")]
    public bool hasAmount = false;
    public int itemAmount = 0;
}
