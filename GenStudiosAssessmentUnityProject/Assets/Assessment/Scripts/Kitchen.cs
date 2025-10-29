using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Kitchen : MonoBehaviour
{
    public EItemType spawnItemType;

    CharacterBase currentInteractingCharacter;

    // cache remaining action time if action is stopped halfway
    float remainingActionTime;

    void OnTriggerEnter(Collider other)
    {
        currentInteractingCharacter = other.GetComponent<CharacterBase>();

        RunActionTimer();
    }

    void OnTriggerExit(Collider other)
    {
        currentInteractingCharacter = other.GetComponent<CharacterBase>();

        // should only run on player for now
        if (currentInteractingCharacter is PlayerController playerController)
        {
            // The currentInteractingCharacter is successfully cast to PlayerController
            // Now can use the playerController object safely
            remainingActionTime = playerController.StopActionTimer();
        }
    }

    void SpawnItem()
    {
        // reset remaining action timer 
        remainingActionTime = 0;
        ItemBase spawnedItem = GameManager.Instance.spawner.SpawnItem(spawnItemType);
        bool addSuccess = currentInteractingCharacter.AddItemToHand(spawnedItem);
        // handle on adding time to hand success
        if (addSuccess)
        {
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
            playerController.StartActionTimer(GetActionSecondsBasedOnType(),
            () =>
            {
                SpawnItem();
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
    public float GetActionSecondsBasedOnType()
    {
        if(remainingActionTime > 0)
        {
            return remainingActionTime;
        }

        return spawnItemType switch
        {
            EItemType.None => -1,
            EItemType.Burger => GameManager.Instance.gameData.secondsToMakeBurger,
            EItemType.SoftDrink => GameManager.Instance.gameData.secondsToMakeSoftDrink,
            _ => -1
        };
    }

}
