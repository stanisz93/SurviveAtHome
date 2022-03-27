using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Courutines {Moving, Attacking, Rushing};

public class OpponentNavigationCoroutines : MonoBehaviour
{
    // Start is called before the first frame update
    public float smoothing = 7f;

    private NavMeshAgent agent;
    private string currentTask;
    private bool currentCanBeDisturbed = true;
    private OpponentActions opponentActions;
    private GameObject trackedObj;
    private Vector3 _target;


    
    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        opponentActions = GetComponent<OpponentActions>();
    }
    

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }

    public void TryToForceTask(string task, bool disturbable, GameObject tracked=null)
    {
        if (currentCanBeDisturbed)
        {
            if(tracked != null)
                trackedObj = tracked;
            Debug.Log("Disturbed");
            CreateTask(task, disturbable);
        }
    }
    private void CreateTask(string task, bool dist)
    {
            StopCoroutine(currentTask);
            currentTask = task;
            currentCanBeDisturbed = dist;
            Debug.Log($"Run task: {currentTask}");
            StartCoroutine(currentTask);
    }
    

    IEnumerator Movement()
    {
        bool targetIsSet = false;
        while(Vector3.Distance(transform.position, _target) > 0.05f)
        {
            opponentActions.SetOpponentMode(OpponentMode.Exploring);
            agent.destination = _target;
            targetIsSet = true;

            yield return null; 
        }
    }

    public IEnumerator RushingToAttack()
    {
        // bool targetIsSet = false;
;       if(trackedObj == null)
            Debug.Log("There is no object to track!");
        
        while(Vector3.Distance(transform.position, trackedObj.transform.position) > opponentActions.stoppingDistance)
        {
            opponentActions.SetOpponentMode(OpponentMode.Rushing);
            agent.destination = trackedObj.transform.position;
            // targetIsSet = true;
            yield return null;
        }
        // while(Vector3.Distance(transform.position, target) < 0.05f)
        //     {
        opponentActions.SetOpponentMode(OpponentMode.Attacking);
                // yield return null;
            // }
        // while(Vector3.Distance(transform.position, target) > 0.05f)
        //     yield return null;
        // opponentActions.SetOpponentMode(OpponentMode.Attacking);
        // yield return null;
    }

  
    
}
