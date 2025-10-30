using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyStack : MonoBehaviour
{
    // object pooling
    List<GameObject> spawnedMoney = new List<GameObject>();

    SO_GameData gameData;
    int currentLastActiveIndex = -1;

    void Start()
    {
        gameData = GameManager.Instance.gameData;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        int moneyUnitCount = currentLastActiveIndex + 1;
        // should only run on player for now
        if (playerController != null && moneyUnitCount > 0)
        {
            // The currentInteractingCharacter is successfully cast to PlayerController
            // Now can use the playerController object safely
            playerController.AddMoney(moneyUnitCount * gameData.oneMoneyUnitValue);
            // ToDo:: add fancy animation or particle effect as feedback
            DeInitializeAllMoney();
        }
    }

    public void SpawnMoney(int amount)
    {
        for (int i = 0; i < amount; ++i)
        {
            SpawnOneUnitMoney();
        }
    }
    
    void DeInitializeAllMoney()
    {
        currentLastActiveIndex = -1;

        foreach (GameObject element in spawnedMoney)
        {
            element.SetActive(false);
        }
    }

    void SpawnOneUnitMoney()
    {
        currentLastActiveIndex += 1;

        // instead of linQ find, just cache last active index since the money will be all deactivate together once collect
        if (currentLastActiveIndex >= spawnedMoney.Count)
        {
            // doesn't exist in pool, so need to create a new one and make this gameobject as parent
            GameObject itemObject = Instantiate(gameData.moneyPrefab, transform);
            // cache it in pool
            spawnedMoney.Add(itemObject);
            InitializeOneUnitMoney(itemObject);
            return;
        }
        
        // exist in the pool, but not active. Use and initialize it.
        InitializeOneUnitMoney(spawnedMoney[currentLastActiveIndex]);
    }

    void InitializeOneUnitMoney(GameObject obj)
    {
        // Get the index of the object in the list
        int indexInList = spawnedMoney.IndexOf(obj);

        // Get the number of columns, rows, and other properties dynamically
        Vector2Int rowsAndColumns = gameData.stackMoneyRowAndColumn;

        // Calculate the row and column based on the index
        int row = (indexInList / rowsAndColumns.y) % rowsAndColumns.x; // Integer division (gives row number)
        int column = indexInList % rowsAndColumns.y;      // Modulo (gives column number)
        int yOffset = indexInList / (rowsAndColumns.x * rowsAndColumns.y);

        // Calculate the position based on row, column, and gap
        Vector3 newPosition = new Vector3(
            column * gameData.gapBetweenStackMoney.x,
            yOffset * gameData.gapBetweenStackMoney.y,
            row * gameData.gapBetweenStackMoney.z
        );

        // Set the new position for the object
        obj.transform.localPosition = newPosition;

        obj.SetActive(true);
    }
}
