using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dumpster : MonoBehaviour
{
    CharacterBase currentInteractingCharacter;

    void OnTriggerEnter(Collider other)
    {
        currentInteractingCharacter = other.GetComponent<CharacterBase>();

        // should only run on player for now
        if (currentInteractingCharacter is not PlayerController)
        {
            currentInteractingCharacter = null;
            return;
        }

        // remove the item on hand
        ItemBase item = currentInteractingCharacter.GetItemOnHand();
        item.Deinitialize();
    }
}
