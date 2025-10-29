using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    // cacheItemData
    SO_ItemData itemData;

    public EItemType ItemType => itemData.itemType;

    public int GetItemValue()
    {
        return itemData.value;
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
    }

    // return to object pool
    public void Deinitialize()
    {
        gameObject.SetActive(false);
    }
    
    public void CreateItem(EItemType itemType)
    {
        itemData = GameManager.Instance.gameData.allItemData.Find(x => x.itemType == itemType);

        if (itemData == null)
        {
            Debug.LogError($"Cannot find item data with type: {itemType} in game data!");
            return;
        }

        gameObject.name = itemData.itemName;

        // create the prefab as child object
        GameObject meshObject = Instantiate(itemData.itemPrefab, transform);
        // reset the position relative to the parent
        meshObject.transform.localPosition = Vector3.zero;
    }
}
