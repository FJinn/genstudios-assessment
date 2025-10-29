using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public enum EMood
    {
        None = 0,
        Happy = 1,
        Angry = 2
    }

    [SerializeField] protected SO_CharacterData characterData;
    [SerializeField] protected Rigidbody body;
    [SerializeField] protected EmojiBubble emojiBubble;
    [SerializeField] Transform handTransform;

    protected EMood currentMood;

    public Stack<ItemBase> itemsStack = new Stack<ItemBase>();

    bool CanAddItemToHand()
    {
        return itemsStack.Count >= characterData.maxCarryItemAmount;
    }

    bool CanAddItemTypeToHand(EItemType itemType)
    {
        if (!characterData.allowItemTypeToBeCarried.Exists(x => x == itemType))
        {
            return false;
        }

        return characterData.itemCarryRule switch
        {
            EItemCarryRule.None => true,
            EItemCarryRule.OneType => (itemsStack.Count > 0 && itemsStack.Peek().ItemType == itemType) || itemsStack.Count == 0,
            _ => true
        };
    }

    public bool AddItemToHand(ItemBase item)
    {
        // either add item to hand evaluation is cannot, return false
        if (!CanAddItemToHand() || !CanAddItemTypeToHand(item.ItemType))
        {
            return false;
        }

        item.transform.position = handTransform.position;
        item.transform.rotation = handTransform.rotation;

        itemsStack.Push(item);

        return true;
    }
    
    public ItemBase GetItemOnHand()
    {
        return itemsStack.Pop();
    }
}
