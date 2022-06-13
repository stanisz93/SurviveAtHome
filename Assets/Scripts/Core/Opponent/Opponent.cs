using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class Opponent : MonoBehaviour
{

    public int damage = 60;

    public Transform zombieMesh;
    private OpponentActions opponentActions;
    private NavMeshAgent agent;
    // private Task performTask;
    private OpponentUtils opponentUtils;
    private VisionFieldOfView vfov;
    private TaskManager taskManager;

    public delegate void InterruptTask();

    public Action<Opponent> OnKill;
    InterruptTask interruptTask;
    private OpponentMagnet opponentMagnet;
   
    

    // Start is called before the first frame updatePc
    void Start()
    {   
        agent = GetComponent<NavMeshAgent>();
        opponentActions = GetComponent<OpponentActions>();
        opponentUtils = GetComponent<OpponentUtils>();
        taskManager = GetComponentInChildren<TaskManager>();
        vfov = GetComponentInChildren<VisionFieldOfView>();
        opponentMagnet = GameObject.FindWithTag("Player").GetComponentInChildren<OpponentMagnet>();
        OnKill += opponentMagnet.RemoveDiedOpponent;

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

    public void GotPushed(Transform transform, float pushForce, float pushTime, bool superHit=false)
    {
        if(taskManager.GetCurrentPriority() > 1) 
            taskManager.ForceToRun(opponentActions.GotPushed(transform, pushForce, pushTime, superHit), 1);
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

    

    public void SetDamagePosition(Vector3 position)
    {
        opponentActions.SetCurrentEffectPosition(position);
    }

    public void SetDefaultDamagePosition()
    {
        opponentActions.SetDefaultEffectPosition();
    }

    // Update is called once per frame

    void Update() {

        var currPriority = taskManager.GetCurrentPriority();
        bool busy = taskManager.TaskIsEmpty();
        if(taskManager.isOpenForTask)
        {
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

    private void OnCollisionEnter(Collision other) {
        Opponent opponent = other.gameObject.GetComponent<Opponent>();
        if(opponent != null)
        {
            ContactPoint contact = other.contacts[0];
            opponent.GotTackled(contact.normal);
        }
    }
    
}

