using System.Collections.Generic;
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
    [SerializeField] protected Animator animator;
    [SerializeField] Transform handTransform;

    protected int isWalkID = Animator.StringToHash("IsWalk");
    protected int isAngryID = Animator.StringToHash("IsAngry");
    protected int isCookingID = Animator.StringToHash("IsCooking");
    protected int isCarryIdleID = Animator.StringToHash("IsCarryIdle");
    protected int isCarryWalkID = Animator.StringToHash("IsCarryWalk");

    protected EMood currentMood;
    // use list instead of stack for flexibiliity, such as able to put second item on counter
    protected List<ItemBase> itemsStack = new List<ItemBase>();

    bool CanAddItemToHand()
    {
        return itemsStack.Count < characterData.maxCarryItemAmount;
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
            EItemCarryRule.OneType => (itemsStack.Count > 0 && itemsStack[0].ItemType == itemType) || itemsStack.Count == 0,
            _ => true
        };
    }

    public bool EvaluateAddItemToHand(EItemType itemType)
    {
        return CanAddItemToHand() && CanAddItemTypeToHand(itemType);
    }

    public bool AddItemToHand(ItemBase item)
    {
        // either add item to hand evaluation is cannot, return false
        if (!EvaluateAddItemToHand(item.ItemType))
        {
            Debug.LogWarning($"Failed to add item to hand! Due to: Cannot add item = {!CanAddItemToHand()} or Cannot add item type = {!CanAddItemTypeToHand(item.ItemType)}!");
            return false;
        }

        itemsStack.Add(item);

        item.transform.position = handTransform.position + Vector3.up * GameManager.Instance.gameData.carryItemGapDistance * itemsStack.Count;
        item.transform.rotation = handTransform.rotation * Quaternion.Euler(-90, 0, 90);
        item.transform.SetParent(handTransform);

        animator.SetBool(isCarryIdleID, true);

        return true;
    }

    public ItemBase GetItemOnHand(EItemType specificItem = EItemType.None)
    {
        ItemBase targetItem;
        if (specificItem != EItemType.None)
        {
            targetItem = itemsStack.Find(x => x.ItemType == specificItem);
        }
        else // get last item if not specific
        {
            targetItem = itemsStack.Count == 0 ? null : itemsStack[^1];
        }
        if (targetItem != null)
        {
            itemsStack.Remove(targetItem);

            animator.SetBool(isCarryIdleID, itemsStack.Count > 0);
        }
        return targetItem;
    }

    public List<ItemBase> GetAllItemsOnHand()
    {
        return itemsStack;
    }
    
    protected void SetWalkAnimation(bool active)
    {
        animator.SetBool(isCarryWalkID, active? itemsStack.Count > 0 : false);
        animator.SetBool(isWalkID, active? itemsStack.Count == 0 : false);
    }
}
