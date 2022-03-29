using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Opponent : MonoBehaviour
{
    public bool debug = false;

    public int damage = 20;

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
        
        if(Input.GetMouseButtonDown(0))
        {
            taskManager.ForceToRun(opponentActions.WalkFollowMousePosition());
        }
        else if (vfov.visibleTargets.Count == 1 && opponentActions.currentAction == ActionType.AllowToStop)
            {
                taskManager.ForceToRun(opponentActions.AgentAttack(vfov.visibleTargets[0], damage));
            }
        else if(taskManager.TaskIsEmpty())
        {
            {
                taskManager.ForceToRun(opponentActions.Exploring());
            }
        }
    }
    
}

