using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SO_GameData gameData;
    public Spawner spawner;

    public Transform customerSpawnParentTransform;
    public Transform customerSpawnTransform;
    public Transform customerExitTransform;

    // Static instance for the singleton pattern
    private static GameManager instance;
    // Public property to access the instance
    public static GameManager Instance {get { return instance;}}

    // Prevent duplicate instances
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);  // Destroy duplicate instance
        }
        else
        {
            instance = this;  // Set the current instance
        }
    }
}
