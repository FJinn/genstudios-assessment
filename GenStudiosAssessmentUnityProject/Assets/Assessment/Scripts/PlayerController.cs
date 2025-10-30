using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase
{
    [SerializeField] RectTransform joystickBase;
    [SerializeField] RectTransform joystickHandle;
    [SerializeField] Transform rigTransform;

    Vector2 touchStartPosition;
    Vector2 touchCurrentPosition;
    Vector2 joystickInput;
    
    bool isTouching = false;
    bool isMouseActive = false;

    int moneyAmount;
    public static Action<int> onMoneyChanged;

    float currentActionTimerElapsedTime;
    Coroutine actionTimerRoutine;

    // since there is only one action timer at a time, we can just use this method
    /// <summary>
    /// Call to run action
    /// </summary>
    /// <param name="time"></param>
    /// <param name="actionTimerUI">probably can have a better structure for ui update instead of here</param>
    /// <param name="onCompleted">if return false, means the action is failed</param>
    public void StartActionTimer(float initialTime, float targetTime, ActionTimerUI actionTimerUI, Action onCompleted)
    {
        if (actionTimerRoutine != null)
        {
            StopCoroutine(actionTimerRoutine);
        }
        actionTimerRoutine = StartCoroutine(ActionTimerUpdate(initialTime, targetTime, actionTimerUI, onCompleted));
    }

    /// <summary>
    /// stop current action timer
    /// </summary>
    /// <returns>remaining time in seconds; return -1 if no action timer running</returns>
    public float StopActionTimer()
    {
        if (actionTimerRoutine != null)
        {
            StopCoroutine(actionTimerRoutine);
            animator.SetBool(isCookingID, false);
            return currentActionTimerElapsedTime;
        }
        // if there is no action timer running
        return -1;
    }

    IEnumerator ActionTimerUpdate(float initialTime, float targettime, ActionTimerUI actionTimerUI, Action onCompleted)
    {
        currentActionTimerElapsedTime = initialTime;
        actionTimerUI?.RunToDo();

        animator.SetBool(isCookingID, true);

        while (currentActionTimerElapsedTime < targettime)
        {
            actionTimerUI?.UpdateToDoFillValue(currentActionTimerElapsedTime / targettime);
            currentActionTimerElapsedTime += Time.deltaTime;

            yield return null;
        }

        animator.SetBool(isCookingID, false);
        
        currentActionTimerElapsedTime = 0;
        onCompleted?.Invoke();
    }

    public void AddMoney(int value)
    {
        moneyAmount += value;
        onMoneyChanged?.Invoke(moneyAmount);
    }

    public int GetMoneyAmount()
    {
        return moneyAmount;
    }

    void Update()
    {
        // Check if mouse or touch input is active
        HandleInput();

        // Apply joystick input to player movement
        MovePlayer();
    }

    // maybe can use new input system instead of this
    private void HandleInput()
    {
        // Check if the mouse is pressed or if there's any touch input
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            if (Input.touchCount > 0)
            {
                // Handle touch input
                Touch touch = Input.GetTouch(0);
                ProcessTouch(touch);
            }
            else if (Input.GetMouseButton(0))
            {
                // Handle mouse input (simulating touch with mouse)
                ProcessMouseInput();
            }
        }
        else
        {
            // Reset on touch or mouse button release
            if (isTouching || isMouseActive)
            {
                isTouching = false;
                isMouseActive = false;
                joystickBase.gameObject.SetActive(false); // Hide joystick
                joystickHandle.position = touchStartPosition; // Reset handle
                joystickInput = Vector3.zero; // reset value to stop cahracter movement
            }
        }
    }

    private void ProcessTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            // Touch started
            isTouching = true;
            touchStartPosition = touch.position;
            joystickBase.gameObject.SetActive(true); // Show joystick
            joystickBase.position = touch.position; // Place base at touch position
            joystickHandle.position = touch.position; // Place handle at touch position
        }
        else if (touch.phase == TouchPhase.Moved && isTouching)
        {
            // Touch moved
            touchCurrentPosition = touch.position;
            Vector2 direction = touchCurrentPosition - touchStartPosition;
            joystickInput = direction.normalized;

            // Move joystick handle according to touch
            joystickHandle.position = touchStartPosition + joystickInput * 50f; // 50 is the radius of movement
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            // Touch ended
            isTouching = false;
            joystickBase.gameObject.SetActive(false); // Hide joystick
            joystickHandle.position = touchStartPosition; // Reset handle
        }
    }

    private void ProcessMouseInput()
    {
        // Get the mouse position
        Vector2 mousePosition = Input.mousePosition;

        if (!isMouseActive)
        {
            // Mouse clicked
            isMouseActive = true;
            touchStartPosition = mousePosition;
            joystickBase.gameObject.SetActive(true); // Show joystick
            joystickBase.position = mousePosition; // Place base at mouse position
            joystickHandle.position = mousePosition; // Place handle at mouse position
        }

        // Update joystick position
        if (isMouseActive)
        {
            touchCurrentPosition = mousePosition;
            Vector2 direction = touchCurrentPosition - touchStartPosition;
            joystickInput = direction.normalized;

            // Move joystick handle according to mouse movement
            joystickHandle.position = touchStartPosition + joystickInput * 50f; // 50 is the radius of movement
        }
    }

    private void MovePlayer()
    {
        SetWalkAnimation(!(joystickInput.x == 0 && joystickInput.y == 0));
        Vector3 joyStickInputVector = new Vector3(-joystickInput.x, 0f, -joystickInput.y);
        // Using joystick input to move the player, invert the value so movement direction is same with joystick handle movement
        Vector3 movement = joyStickInputVector * characterData.moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        if(Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            Quaternion targetRotation = Quaternion.LookRotation(joyStickInputVector);
            rigTransform.rotation = Quaternion.Slerp(rigTransform.rotation, targetRotation, Time.deltaTime * 10f);
        }

    }
}
