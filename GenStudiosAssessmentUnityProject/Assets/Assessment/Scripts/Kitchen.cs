using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Kitchen : MonoBehaviour
{
    [SerializeField] EItemType spawnItemType;
    [SerializeField] ActionTimerUI actionTimerUI;

    CharacterBase currentInteractingCharacter;

    // cache remaining action time if action is stopped halfway
    float remainingActionTime;
    // item spawned on kitchen, for now assume only 1 at a time
    ItemBase spawnedItem;

    void OnTriggerEnter(Collider other)
    {
        currentInteractingCharacter = other.GetComponent<CharacterBase>();

        // should only run on player for now
        if (currentInteractingCharacter is not PlayerController)
        {
            currentInteractingCharacter = null;
            return;
        }

        AddItemToPlayer();

        // it shouldn't run if player hand is full
        // if item has been spawned, it should be on player hand already
        // AddItemToHand();
    }

    void AddItemToPlayer()
    {
        if (spawnedItem == null && currentInteractingCharacter.EvaluateAddItemToHand(spawnItemType))
        {
            RunActionTimer();
        }
    }
    

    void OnTriggerExit(Collider other)
    {
        currentInteractingCharacter = other.GetComponent<CharacterBase>();

        // should only run on player for now
        if (currentInteractingCharacter is not PlayerController playerController)
        {
            currentInteractingCharacter = null;
            return;
        }
        // The currentInteractingCharacter is successfully cast to PlayerController
        // Now can use the playerController object safely
        remainingActionTime = playerController.StopActionTimer();

        if (GameManager.Instance.gameData.allItemData.Find(x => x.itemType == spawnItemType).resetSpawnSecondsIfFailed)
        {
            actionTimerUI.Deactivate();
        }
        
        currentInteractingCharacter = null;
    }

    void SpawnItem()
    {
        // reset remaining action timer 
        remainingActionTime = 0;
        spawnedItem = GameManager.Instance.spawner.SpawnItem(spawnItemType);
    }

    void AddItemToHand()
    {
        bool addSuccess = currentInteractingCharacter.AddItemToHand(spawnedItem);
        // handle on adding time to hand success
        if (addSuccess)
        {
            // reset spawned item since it is not in player hand
            spawnedItem = null;
            // action timer ui should be cleaned 
            actionTimerUI.Deactivate();
            // ToDo:: maybe some visual/audio feedback
            return;
        }
        // handle on adding time to hand fail
        spawnedItem.Deinitialize();
    }

    public void RunActionTimer()
    {
        // should only run on player for now
        if (currentInteractingCharacter is PlayerController playerController)
        {
            // The currentInteractingCharacter is successfully cast to PlayerController
            // Now can use the playerController object safely
            playerController.StartActionTimer(
            GetInitialActionTime(),
            GetActionSecondsBasedOnType(),
            actionTimerUI,
            () =>
            {
                SpawnItem();
                actionTimerUI.DoneAction();
                // add item to hand, character should still be interactive area, else the timer will be stopped and this will not be called
                AddItemToHand();

                AddItemToPlayer();
            });
        }
        else
        {
            // Handle case where the cast fails (e.g., log an error)
            Debug.LogError("The interacting character is not a PlayerController.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>-1 if the type is None</returns>
    float GetActionSecondsBasedOnType()
    {
        return spawnItemType switch
        {
            EItemType.None => -1,
            EItemType.Burger => GameManager.Instance.gameData.allItemData.Find(x => x.itemType == EItemType.Burger).secondsToSpawn,
            EItemType.SoftDrink => GameManager.Instance.gameData.allItemData.Find(x => x.itemType == EItemType.SoftDrink).secondsToSpawn,
            _ => -1
        };
    }

    float GetInitialActionTime()
    {
        return spawnItemType switch
        {
            EItemType.None => 0,
            EItemType.Burger => GameManager.Instance.gameData.allItemData.Find(x => x.itemType == EItemType.Burger).resetSpawnSecondsIfFailed ? 0 : remainingActionTime,
            EItemType.SoftDrink => GameManager.Instance.gameData.allItemData.Find(x => x.itemType == EItemType.SoftDrink).resetSpawnSecondsIfFailed ? 0 : remainingActionTime,
            _ => 0
        };
    }
}
