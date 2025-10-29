using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase
{
    int moneyAmount;

    float currentActionTimerElapsedTime;
    Coroutine actionTimerRoutine;

    // since there is only one action timer at a time, we can just use this method
    /// <summary>
    /// Call to run action
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onCompleted">if return false, means the action is failed</param>
    public void StartActionTimer(float time, Action onCompleted)
    {
        if (actionTimerRoutine != null)
        {
            StopCoroutine(actionTimerRoutine);
        }
        actionTimerRoutine = StartCoroutine(ActionTimerUpdate(time, onCompleted));
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
            return currentActionTimerElapsedTime;
        }
        // if there is no action timer running
        return -1;
    }

    IEnumerator ActionTimerUpdate(float time, Action onCompleted)
    {
        float currentActionTimerElapsedTime = 0;

        while (currentActionTimerElapsedTime < time)
        {
            currentActionTimerElapsedTime += Time.deltaTime;

            yield return null;
        }

        onCompleted?.Invoke();
    }

    public void AddMoney(int value)
    {
        moneyAmount += value;
    }

    public int GetMoneyAmount()
    {
        return moneyAmount;
    }
}
