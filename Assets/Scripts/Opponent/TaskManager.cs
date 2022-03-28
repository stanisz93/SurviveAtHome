using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {
    


    private IEnumerator currentTask = null;
    private bool isEmpty = true;


    private void StartTask(IEnumerator task)
    {
        currentTask = task;
        StartCoroutine(currentTask);
        isEmpty = false;
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
            StopCoroutine(currentTask);
            isEmpty = true;
        }
    }

    public void ForceToRun(IEnumerator task)
    {
        StopCurrentTask();
        StartTask(task);
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
