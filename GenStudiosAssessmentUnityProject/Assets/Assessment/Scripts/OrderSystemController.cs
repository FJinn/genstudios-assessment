using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSystemController : MonoBehaviour
{
    [SerializeField] Transform moneyParentTransform;

    // customer queue
    Queue<Character_Customer> customerQueue = new Queue<Character_Customer>();
    // cache current customer
    Character_Customer currentCustomer;
    // items on counter, should act like a queue
    public Queue<ItemBase> itemsOnCounter;

    Coroutine customerAngryRoutine;
    // cache seconds to be angry 
    WaitForSeconds secondsToBeAngry;

    Coroutine foodOnTableBeforeServeRoutine;
    // cache seconds to allow customer to take food 
    WaitForSeconds secondsToAllowTakeFood;

    // cache game data
    SO_GameData gameData;

    // cache interacting character
    CharacterBase currentInteractingCharacter;

    void Start()
    {
        gameData = GameManager.Instance.gameData;
        secondsToBeAngry = new WaitForSeconds(gameData.secondsForCustomerToGetAngry);
        secondsToAllowTakeFood = new WaitForSeconds(gameData.secondsForCustomerToTakeFood);
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
            ItemBase item = playerController.GetItemOnHand();
            if(item == null)
            {
                return;
            }
            AddItemToCounter(item);
        }
    }

    /// <summary>
    /// add customer to queue
    /// </summary>
    /// <param name="customer"></param>
    /// <returns>true if success</returns>
    public bool AddCustomerToQueue(Character_Customer customer)
    {
        if (customerQueue.Count >= gameData.maxCustomerInQueue) return false;

        customerQueue.Enqueue(customer);
        return true;
    }

    void CustomerPayMoney(ItemBase item)
    {
        // Money received from customer should be placed at the SampleScene/Counter/Money position. 2x3 grid arrangement and stacked upward.
    
    }

    /// <summary>
    /// add food to counter
    /// </summary>
    /// <param name="item"></param>
    public void AddItemToCounter(ItemBase item)
    {
        itemsOnCounter.Enqueue(item);

        // let food stays for awhile before customer takes it
        RunFoodStayOnCounter();
    }

    // this will run every time an item is add onto counter
    public void CustomerTakesItemFromCounter()
    {
        ItemBase item = itemsOnCounter.Peek();
        // check if it is the correct order, if not, ignore it
        if (item.ItemType != currentCustomer.GetOrder()) return;
        // remove item
        currentCustomer.TakeItem(itemsOnCounter.Dequeue());
        CustomerPayMoney(item);

        currentCustomer.MoveToTargetTransform(GameManager.Instance.customerExitTransform, true);
    }

    // set current serving customer
    public void UpdateCurrentCustomer()
    {
        currentCustomer = customerQueue.Dequeue();
        // trigger customer order initialization
        currentCustomer.InitializeOrder();
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
