using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Customer : CharacterBase
{
    [SerializeField] List<GameObject> outfits;
    static int outfitIndex = 0;

    EItemType itemOrder;

    Vector3 targetPosition;
    //cache
    Coroutine moveRoutine;

    // only considered leave queue when it is deinitialize
    bool inQueue;

    void IncreaseOutfitIndex()
    {
        outfitIndex = (outfitIndex + 1) % outfits.Count;
    }

    /// <summary>
    /// Call this when it is spawned
    /// </summary>
    public void Initialize()
    {
        // cycle through outfit and set active if it is the correct index
        for (int i = 0; i < outfits.Count; ++i)
        {
            outfits[i].SetActive(i == outfitIndex);
        }

        // increase outfit index
        IncreaseOutfitIndex();

        currentMood = EMood.None;
        inQueue = false;

        emojiBubble.Initialize();

        gameObject.SetActive(true);
    }

    void Deinitialize()
    {
        inQueue = false;
        foreach (ItemBase element in itemsStack)
        {
            element.Deinitialize();
        }
        itemsStack.Clear();

        animator.SetBool(isCarryIdleID, false);
        
        emojiBubble.HideEmoji();
        gameObject.SetActive(false);
    }

    public void SetInQueue(bool isInQueue)
    {
        inQueue = isInQueue;
    }

    public bool GetInQueue()
    {
        return inQueue;
    }

    public EItemType GetOrder()
    {
        return itemOrder;
    }

    public void InitializeOrder()
    {
        // randomly pick item for now
        // 0 is none, so have to choose between 1 or 2
        itemOrder = Random.Range(0, 1f) >= 0.5f ? EItemType.Burger : EItemType.SoftDrink;

        emojiBubble.ShowOrder(true, GameManager.Instance.gameData.allItemData.Find(x => x.itemType == itemOrder).itemName);
    }

    public void InitializeAngry()
    {
        currentMood = EMood.Angry;
        emojiBubble.ShowEmoji(currentMood);

        animator.SetBool(isAngryID, true);
    }

    public void TakeItem(ItemBase item)
    {
        // When the customer have taken the food and paid for it, it will then leaves the counter and display a happy speech bubble
        InitializeHappy();

        itemOrder = EItemType.None;

        AddItemToHand(item);
    }

    void InitializeHappy()
    {
        animator.SetBool(isAngryID, false);
        currentMood = EMood.Happy;
        emojiBubble.ShowEmoji(currentMood);
    }

    public void MoveToTargetTransform(Vector3 targetPos, bool deinitializeWhenDone = false)
    {
        // can have better structure such as on mood changed, we change the animator bool
        animator.SetBool(isAngryID, false);

        targetPosition = targetPos;

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(MoveUpdate(deinitializeWhenDone));
    }

    IEnumerator MoveUpdate(bool deinitializeWhenDone)
    {
        SetWalkAnimation(true);
        
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f) // Continue moving while we're not very close to the target
        {
            // Move towards the target at the specified speed
            float step = characterData.moveSpeed * Time.deltaTime; // Calculate how far to move this frame
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation

            // Yield return null to continue the movement in the next frame
            yield return null;
        }

        // Ensure we set the final position exactly to the target
        transform.position = targetPosition;

        SetWalkAnimation(false);
        
        if (deinitializeWhenDone)
        {
            Deinitialize();
        }
    }
    
    public Vector3 GetBackPositionInQueue()
    {
        SO_GameData gameData = GameManager.Instance.gameData;

        // Calculate the final position while keeping the Y value the same
        Vector3 finalPosition = new Vector3(
            transform.position.x + (transform.forward * -1).x * gameData.queueGapDistance,
            transform.position.y,  // Keep Y constant
            transform.position.z + (transform.forward * -1).z * gameData.queueGapDistance
        );

        return finalPosition;
    }
}
