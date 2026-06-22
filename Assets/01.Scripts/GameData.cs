using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameDataBase
{
    public string Id;
}

[System.Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string Description;
    public string ItemType;
    public int Heal;
    public float Speed;
    public int MaxStackCount;
    public int SellingPrice;
    public int PurchasePrice;
    public string IconPath;
    public string PrefabPath;
}