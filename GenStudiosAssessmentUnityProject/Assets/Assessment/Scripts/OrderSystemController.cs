using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrderSystemController : MonoBehaviour
{
    [SerializeField] Transform moneyParentTransform;
    [SerializeField] Transform queueStartingTransform;
    [SerializeField] ItemSpaceOnCounter itemAPoint;
    [SerializeField] ItemSpaceOnCounter itemBPoint;
    [SerializeField] MoneyStack moneyStack;

    [Serializable]
    class ItemSpaceOnCounter
    {
        public Transform spaceTransform;
        public ItemBase holdingItem { private set; get; }
        
        public void SetItem(ItemBase newItem)
        {
            holdingItem = newItem;
            if(newItem != null)
            {
                newItem.transform.position = spaceTransform.position;
                newItem.transform.rotation = spaceTransform.rotation;
            }
        }
    }

    // customer queue, sue list instead of queue for more flexibility
    List<Character_Customer> customerQueue = new List<Character_Customer>();
    // cache current customer
    Character_Customer currentCustomer;
    // items on counter, should act like a queue
    public List<ItemBase> itemsOnCounter = new List<ItemBase>();

    Coroutine customerAngryRoutine;
    // cache seconds to be angry 
    WaitForSeconds secondsToBeAngry;

    Coroutine foodOnTableBeforeServeRoutine;
    // cache seconds to allow customer to take food 
    WaitForSeconds secondsToAllowTakeFood;

    // delay to get new customer in queue
    Coroutine delayToGetNewCustomerIntoQueueRoutine;
    WaitForSeconds secondsToGetNewCustomerInQueue;

    // cache game data
    SO_GameData gameData;

    // cache interacting character
    CharacterBase currentInteractingCharacter;

    void Start()
    {
        gameData = GameManager.Instance.gameData;
        secondsToBeAngry = new WaitForSeconds(gameData.secondsForCustomerToGetAngry);
        secondsToAllowTakeFood = new WaitForSeconds(gameData.secondsForCustomerToTakeFood);
        secondsToGetNewCustomerInQueue = new WaitForSeconds(gameData.secondsToGetNewCustomerInQueue);

        // listen to spawn customer
        Spawner.onCustomerSpawned += AddCustomerToQueue;
    }

    void OnDestroy()
    {
        Spawner.onCustomerSpawned -= AddCustomerToQueue;
    }

    void OnTriggerEnter(Collider other)
    {
        currentInteractingCharacter = other.GetComponent<CharacterBase>();

        // should only run on player for now
        if (currentInteractingCharacter is PlayerController playerController)
        {
            // The currentInteractingCharacter is successfully cast to PlayerController
            // Now can use the playerController object safely
            // put item onto table if exist
            // get specific item on hand, so we can avoid situation that no item on counter is customer order
            // probably can redo this if we have a bin feature (& more time) where player can throw away unwanted food
            EItemType targetItemType = EItemType.None;
            // for now, jsut hard coded this, could have better structure if design is more defined or have more time on assessment
            // run twice so player don't need to enter, exit and reenter to add multiple item onto counter
            if (itemAPoint.holdingItem == null)
            {
                targetItemType = EItemType.Burger;
            }
            // get last item if not specific
            ItemBase item = playerController.GetItemOnHand(targetItemType);
            if (item != null)
            {
                item.transform.SetParent(null);
                AddItemToCounter(item);
            }

            if (itemBPoint.holdingItem == null)
            {
                targetItemType = EItemType.SoftDrink;
            }
            // get last item if not specific
            item = playerController.GetItemOnHand(targetItemType);
            if (item != null)
            {
                item.transform.SetParent(null);
                AddItemToCounter(item);
            }
        }
    }

    /// <summary>
    /// add customer to queue
    /// </summary>
    /// <param name="customer"></param>
    public void AddCustomerToQueue(Character_Customer customer)
    {
        // nothing for now, can add more rules
        if (customerQueue.Count >= gameData.maxCustomerInQueue) return;

        customerQueue.Add(customer);
        customer.SetInQueue(true);
        UpdateCustomerInQueuePosition(customer);

        // call handle customer if this is the first customer
        if (customerQueue.Count == 1)
        {
            UpdateCurrentCustomer();
        }
    }

    void UpdateCustomerInQueuePosition(Character_Customer customer)
    {
        Vector3 backwardDirection = queueStartingTransform.forward;
        int customerIndexInQueue = customerQueue.IndexOf(customer);
        Vector3 targetPosition = queueStartingTransform.position + (gameData.queueGapDistance * customerIndexInQueue * backwardDirection);
        customer.MoveToTargetTransform(targetPosition);
    }

    void CustomerPayMoney(ItemBase item)
    {
        // Money received from customer should be placed at the SampleScene/Counter/Money position. 2x3 grid arrangement and stacked upward.
        moneyStack.SpawnMoney(item.GetItemValue());
    }

    /// <summary>
    /// add food to counter, fail if the counter is full
    /// </summary>
    /// <param name="item"></param>
    public bool AddItemToCounter(ItemBase item)
    {
        // for now hardcoded to set point a can be burger only and point b is soft drink only
        if (itemAPoint.holdingItem == null && item.ItemType == EItemType.Burger)
        {
            itemAPoint.SetItem(item);
        }
        else if(itemBPoint.holdingItem == null && item.ItemType == EItemType.SoftDrink)
        {
            itemBPoint.SetItem(item);
        }
        else // all occupied
        {
            return false;
        }

        itemsOnCounter.Add(item);

        // let food stays for awhile before customer takes it
        RunFoodStayOnCounter();

        return true;
    }

    // this will run every time an item is add onto counter or current customer changed
    public void CustomerTakesItemFromCounter()
    {
        // check if there is customer, if not, ignore it
        if (currentCustomer == null) return;

        ItemBase item = itemsOnCounter.Find(x => x.ItemType == currentCustomer.GetOrder());

        // if not item, ignore it
        if (item == null) return;

        // clear counter space
        if (itemAPoint.holdingItem == item) itemAPoint.SetItem(null);
        if (itemBPoint.holdingItem == item) itemBPoint.SetItem(null);

        // customer takes food = not angry
        StopAngryRoutine();
        // remove item after customer takes it
        currentCustomer.TakeItem(item);
        itemsOnCounter.Remove(item);
        CustomerPayMoney(item);

        currentCustomer.MoveToTargetTransform(GameManager.Instance.customerExitTransform.position, true);

        // customer leaves queue
        customerQueue.Remove(currentCustomer);
        currentCustomer = null;

        // update rest of the customers in queue position
        foreach (Character_Customer element in customerQueue)
        {
            UpdateCustomerInQueuePosition(element);
        }

        if (delayToGetNewCustomerIntoQueueRoutine != null)
        {
            StopCoroutine(delayToGetNewCustomerIntoQueueRoutine);
        }
        delayToGetNewCustomerIntoQueueRoutine = StartCoroutine(DelayToGetNewCustomerIntoQueue());
    }
    
    IEnumerator DelayToGetNewCustomerIntoQueue()
    {
        yield return secondsToGetNewCustomerInQueue;

        PutNotInQueueCustomerIntoQueue();

        // handle next customer
        UpdateCurrentCustomer();
    }
    
    void PutNotInQueueCustomerIntoQueue()
    {
        // get possible customer into queue
        Character_Customer customerNotInQueue = GameManager.Instance.spawner.GetAllCustomers().Find(x => !x.GetInQueue());

        if (customerNotInQueue == null)
        {
            return;
        }
        customerNotInQueue.Initialize();

        AddCustomerToQueue(customerNotInQueue);
    }

    // set current serving customer
    public void UpdateCurrentCustomer()
    {
        // handling first customer
        currentCustomer = customerQueue[0];
        // trigger customer order initialization
        currentCustomer.InitializeOrder();
        // if there is food on counter, check it to see if customer can take it
        if(itemsOnCounter.Count > 0)
        {
            // let food stays for awhile before customer takes it
            RunFoodStayOnCounter();
        }
        RunAngryRoutine();
    }

    void RunAngryRoutine()
    {
        if (customerAngryRoutine != null)
        {
            StopCoroutine(customerAngryRoutine);
        }
        customerAngryRoutine = StartCoroutine(CustomerAngryUpdate());
    }

    void StopAngryRoutine()
    {
        if (customerAngryRoutine != null)
        {
            StopCoroutine(customerAngryRoutine);
        }
    }

    IEnumerator CustomerAngryUpdate()
    {
        yield return secondsToBeAngry;

        currentCustomer.InitializeAngry();
    }

    void RunFoodStayOnCounter()
    {
        if (foodOnTableBeforeServeRoutine != null)
        {
            StopCoroutine(foodOnTableBeforeServeRoutine);
        }
        foodOnTableBeforeServeRoutine = StartCoroutine(FoodStayOnCounterUpdate());
    }

    IEnumerator FoodStayOnCounterUpdate()
    {
        yield return secondsToAllowTakeFood;
        CustomerTakesItemFromCounter();
    }
}
