using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/Character Data", order = 1)]
public class SO_CharacterData : ScriptableObject
{
    public string characterName;
    public ECharacterType characterType;
    public float moveSpeed;
    // carry max x items
    public int maxCarryItemAmount = 1;
    // what item can be carried, just 1 type, or mix or all
    public EItemCarryRule itemCarryRule;
    // what item is allowed
    public List<EItemType> allowItemTypeToBeCarried;
}

public enum ECharacterType
{
    None = 0,
    Player = 1,
    Customer = 2
}

public enum EItemCarryRule
{
    /// <summary>
    /// can carry anything, mix
    /// </summary>
    None = 0,
    OneType = 1
}