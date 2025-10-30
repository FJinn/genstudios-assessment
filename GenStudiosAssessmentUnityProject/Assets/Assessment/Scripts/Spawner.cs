using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // object pooling
    List<Character_Customer> spawnedCustomers = new List<Character_Customer>();
    List<ItemBase> spawnedItems = new List<ItemBase>();

    // notify that customer has spawned
    // for assessment, there is only 1 counter, so don't need to set rules for which counter to go
    public static Action<Character_Customer> onCustomerSpawned;

    Coroutine spawnCustomerRoutine;
    // cache customer spawn time
    WaitForSeconds secondToSpawnCustomer;
    // cache game data
    SO_GameData gameData;

    void Start()
    {
        gameData = GameManager.Instance.gameData;
        secondToSpawnCustomer = new WaitForSeconds(gameData.secondsToSpawnCustomer);

        StartSpawnCustomer();
    }

    public void StartSpawnCustomer()
    {
        if (spawnCustomerRoutine != null)
        {
            StopCoroutine(spawnCustomerRoutine);
        }
        spawnCustomerRoutine = StartCoroutine(SpawnCustomerUpdate());
    }

    // created cause there is a start, there should be a stop
    // probably don't need this for assessment
    public void StopSpawnCustomer()
    {
        if (spawnCustomerRoutine != null)
        {
            StopCoroutine(spawnCustomerRoutine);
        }
    }

    // maybe can create a template
    void SpawnCustomer()
    {
        Character_Customer found = spawnedCustomers.Find(x => !x.gameObject.activeInHierarchy);
        if (found)
        {
            // exist in the pool, but not active. Use and initialize it.
            found.Initialize();
            onCustomerSpawned?.Invoke(found);
            return;
        }

        // doesn't exist in pool, so need to create a new one (this means customers in game amount = list count)
        // but first, check if it hits the game limit of number of customer
        if (spawnedCustomers.Count >= gameData.maxCustomerAmount)
        {
            // spawnedCustomer list count should be same with max customer amount
            return;
        }
        Character_Customer newCustomer = Instantiate(gameData.customerPrefab, GameManager.Instance.customerSpawnParentTransform);
        // set its location
        newCustomer.transform.position = GameManager.Instance.customerSpawnTransform.position;
        // cache it in pool
        spawnedCustomers.Add(newCustomer);
        newCustomer.gameObject.name = "Customer" + spawnedCustomers.Count;
        // don't forget to initialize
        newCustomer.Initialize();
        
        onCustomerSpawned?.Invoke(newCustomer);
    }
    
    // maybe can create a template
    public ItemBase SpawnItem(EItemType itemType)
    {
        ItemBase found = spawnedItems.Find(x => !x.gameObject.activeInHierarchy && x.ItemType == itemType);
        if (found)
        {
            // exist in the pool, but not active. Use and initialize it.
            found.Initialize();
            return found;
        }

        // doesn't exist in pool, so need to create a new one
        GameObject itemObject = new GameObject("New Item");
        ItemBase newItem = itemObject.AddComponent<ItemBase>();
        newItem.CreateItem(itemType);
        // cache it in pool
        spawnedItems.Add(newItem);
        return newItem;
    }

    IEnumerator SpawnCustomerUpdate()
    {
        // ToDo:: set rule for this such as under x conditions spawn will be called and stop instead of this infinite runs
        while (true)
        {
            SpawnCustomer();
            yield return secondToSpawnCustomer;
        }
    }

    public List<Character_Customer> GetAllCustomers()
    {
        return spawnedCustomers;
    }
}
