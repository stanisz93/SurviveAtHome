using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {
    


    private IEnumerator currentTask = null;
    private bool isEmpty = true;
    private bool externalBlock = false; //usefull when some sub functionality hasnt finished
    
    private static int lowestPriority = 5;
    private int currentPriority = lowestPriority;

    private IEnumerator StartTask(IEnumerator task, int prior)
    {
        currentTask = task;
        isEmpty = false;
        StartCoroutine(currentTask);
        currentPriority = prior;
        return currentTask;
    }

    public bool TaskIsEmpty()
    {
        return isEmpty;
    }

    public void LockEndOfTask()
    {
        externalBlock = true;
    }
    public void UnlockEndOfTask()
    {
        externalBlock = false;
    }

    public IEnumerator WaitForReleaseLock()
    {
        while(externalBlock)
        {
            yield return null;
        }
    }


    public void TaskSetToFinish()
    {
            isEmpty = true;
            currentPriority = lowestPriority;
    }
    public void StopCurrentTask()
    {
        if(currentTask != null)
        {
            if(GameSystem.Instance.taskManagerDebug)
            {
                Debug.Log($"{currentTask} Task stops.");
            }
            StopCoroutine(currentTask);
            currentPriority = lowestPriority;
            isEmpty = true;
        }
    }

    public IEnumerator ForceToRun(IEnumerator task, int prior)
    {
        StopCurrentTask();
        return StartTask(task, prior);
    }

    public int GetCurrentPriority() { return currentPriority;}

    public void TryToRun(IEnumerator task, out bool succeed, string desc="")
    {
        if(TaskIsEmpty())
        {
            StartTask(task, 3); // prior set to 3!!
            succeed = true;
        }
        else if(GameSystem.Instance.opponentDebug && desc!="")
            Debug.Log($"Tried to run task: {task} named as {desc} but opponent is busy.");
        succeed = false;
    }
    
}
