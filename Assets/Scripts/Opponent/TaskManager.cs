using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {
    


    private IEnumerator currentTask = null;
    private bool isEmpty = true;


    private IEnumerator StartTask(IEnumerator task)
    {
        currentTask = task;
        isEmpty = false;
        StartCoroutine(currentTask);
        return currentTask;
    }

    public bool TaskIsEmpty()
    {
        return isEmpty;
    }

    public void TaskSetToFinish()
    {
        isEmpty = true;
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
            isEmpty = true;
        }
    }

    public IEnumerator ForceToRun(IEnumerator task)
    {
        StopCurrentTask();
        return StartTask(task);
    }

    public bool TryToRun(IEnumerator task, string desc="")
    {
        if(TaskIsEmpty())
        {
            StartTask(task);
            return true;
        }
        else if(GameSystem.Instance.opponentDebug && desc!="")
            Debug.Log($"Tried to run task: {task} named as {desc} but opponent is busy.");
        return false;
    }
    
}
