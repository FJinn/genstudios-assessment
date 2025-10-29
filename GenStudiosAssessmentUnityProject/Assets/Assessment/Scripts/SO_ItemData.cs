using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Item Data", order = 2)]
public class SO_ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemPrefab;
    public EItemType itemType;
    public int value;
}

public enum EItemType
{
    None = 0,
    Burger = 1,
    SoftDrink = 2
}
