using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Opponent : MonoBehaviour
{

    public int damage = 60;

    private OpponentActions opponentActions;
    private NavMeshAgent agent;
    // private Task performTask;
    private OpponentUtils opponentUtils;
    private VisionFieldOfView vfov;
    private TaskManager taskManager;

    public delegate void InterruptTask();
    InterruptTask interruptTask;
   
    

    // Start is called before the first frame update
    void Start()
    {   
        agent = GetComponent<NavMeshAgent>();
        opponentActions = GetComponent<OpponentActions>();
        opponentUtils = GetComponent<OpponentUtils>();
        taskManager = GetComponent<TaskManager>();
        vfov = GetComponentInChildren<VisionFieldOfView>();

    }

    public void Fall()
    {
        if(taskManager.GetCurrentPriority() > 1) 
            taskManager.ForceToRun(opponentActions.Fall(), 1);
    }

    public void GotKicked(Transform transform, float pushForce, float pushTime, float pushDelay)
    {
        if(taskManager.GetCurrentPriority() > 1) 
            taskManager.ForceToRun(opponentActions.GotKicked(transform, pushForce, pushTime, pushDelay), 1);
    }

    // Update is called once per frame

    void Update() {

        var currPriority = taskManager.GetCurrentPriority();
        bool busy = taskManager.TaskIsEmpty();
        if(Input.GetMouseButtonDown(0))
        {
            taskManager.ForceToRun(opponentActions.WalkFollowMousePosition(), 3);
        }
        else if (vfov.FoundedObject() && currPriority > 2)
            {
                taskManager.ForceToRun(opponentActions.AttackSequenceTask(vfov.GetPlayerTarget(), damage), 2);
            }
        else if (vfov.Suspicious() && currPriority > 3)
            {
                taskManager.ForceToRun(opponentActions.CheckSuspiciousPlace(), 3);
            }
        else if(currPriority > 4)
        {
            taskManager.ForceToRun(opponentActions.Exploring(), 4);
        }

    }
    
}

