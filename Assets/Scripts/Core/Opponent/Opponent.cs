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

    

   
    

    // Start is called before the first frame update
    void Start()
    {   
        agent = GetComponent<NavMeshAgent>();
        opponentActions = GetComponent<OpponentActions>();
        opponentUtils = GetComponent<OpponentUtils>();
        taskManager = GetComponent<TaskManager>();
        vfov = GetComponentInChildren<VisionFieldOfView>();

    }


    // Update is called once per frame

    void Update() {

        var currPriority = taskManager.GetCurrentPriority();
        bool busy = taskManager.TaskIsEmpty();
        if(Input.GetMouseButtonDown(0))
        {
            taskManager.ForceToRun(opponentActions.WalkFollowMousePosition(), 1);
    }
        else if (vfov.FoundedObject() && currPriority > 1)
            {
                taskManager.ForceToRun(opponentActions.AgentAttack(vfov.GetPlayerTarget(), damage), 1);
            }
        else if (vfov.Suspicious() && currPriority > 2)
            {
                taskManager.ForceToRun(opponentActions.CheckSuspiciousPlace(), 2);
            }
        else if(currPriority > 3)
        {taskManager.ForceToRun(opponentActions.Exploring(), 3);}

    }
    
}

