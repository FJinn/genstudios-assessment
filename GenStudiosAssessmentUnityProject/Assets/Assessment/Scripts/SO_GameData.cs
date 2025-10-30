using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Data", menuName = "Game/Game Data", order = 3)]
public class SO_GameData : ScriptableObject
{
    /// <summary>
    /// Only X customers may exists at a time
    /// </summary>
    public int maxCustomerAmount = 5;
    /// <summary>
    /// seconds to spawn 1 customer
    /// </summary>
    public float secondsToSpawnCustomer = 2f;
    /// <summary>
    /// If the customer have waited for more than x seconds, display an angry
    /// </summary>
    public float secondsForCustomerToGetAngry = 3f;
    /// <summary>
    /// Only x customers can queue at a time
    /// </summary>
    public int maxCustomerInQueue = 4;
    /// <summary>
    /// Let the food to stay on the table for x before the customer can take it
    /// </summary>
    public float secondsForCustomerToTakeFood = 0.5f;
    /// <summary>
    /// some delay before getting new customer in queue
    /// </summary>
    public float secondsToGetNewCustomerInQueue = 0.5f;

    [Header("Items")]
    public GameObject moneyPrefab;
    public Vector3 gapBetweenStackMoney = new Vector3(0.6f, 0.128f, 0.35f);
    public Vector2Int stackMoneyRowAndColumn = new Vector2Int(2, 3);
    public int oneMoneyUnitValue = 1;

    public List<SO_ItemData> allItemData;

    [Header("Character")]
    public Character_Customer customerPrefab;
    // How long the emoji is shown
    public float secondsToDisplayEmoji = 2f;
    public Sprite happyEmojiSprite;
    public Sprite angryEmojiSprite;
    public Sprite orderSprite;
    public float queueGapDistance = 0.85f;
    public float carryItemGapDistance = 0.5f;

}
