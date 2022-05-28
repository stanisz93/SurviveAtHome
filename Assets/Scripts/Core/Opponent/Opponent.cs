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
        taskManager = GetComponentInChildren<TaskManager>();
        vfov = GetComponentInChildren<VisionFieldOfView>();

    }

    public float GetAgentSpeed()
    {
        return agent.velocity.magnitude;
    }

    public void Fall()
    {
        if(taskManager.GetCurrentPriority() > 1) 
            taskManager.ForceToRun(opponentActions.Fall(), 1);
    }

    public void GotPushed(Transform transform, float pushForce, float pushTime, bool bonusTrigger=false)
    {
        if(taskManager.GetCurrentPriority() > 1) 
            taskManager.ForceToRun(opponentActions.GotPushed(transform, pushForce, pushTime, bonusTrigger), 1);
    }

    public void GotTackled(Vector3 collisionNormal)
    {
        if(taskManager.GetCurrentPriority() > 1) 
            taskManager.ForceToRun(opponentActions.GotTackledWhileRun(collisionNormal), 1);
    }

    public void GotStabbed(Transform player, float pushForce, float pushTime)
    {
        if(taskManager.GetCurrentPriority() > 1) 
            taskManager.ForceToRun(opponentActions.GotStabbed(player, pushForce, pushTime), 1);
    }

    public void SetKickPos(Vector3 position)
    {
        opponentActions.pushEffectPosition.position = position;
    }

    // Update is called once per frame

    void Update() {

        var currPriority = taskManager.GetCurrentPriority();
        bool busy = taskManager.TaskIsEmpty();
        if(Input.GetMouseButtonDown(2))
        {
            taskManager.ForceToRun(opponentActions.WalkFollowMousePosition(), 3);
        }
        else if (vfov.FoundedObject() && currPriority > 2)
            {
                taskManager.ForceToRun(opponentActions.AttackSequenceTask(vfov.GetPlayerTarget()), 2);
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

