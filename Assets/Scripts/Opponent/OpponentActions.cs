using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum OpponentMode {Exploring, Rushing, Attacking, Smelling};
public class OpponentActions : MonoBehaviour
{
    public float walkingSpeed = 0.5f;
    public float runningSpeed = 3.0f;
    private float speed = 1.5f;
    OpponentMode opponentMode = OpponentMode.Exploring;
    private OpponentUtils opponentUtils;
    private NavMeshAgent agent;
    
    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        opponentUtils = GetComponent<OpponentUtils>();
    }

    // Update is called once per frame
    public void AgentAttack(Vector3 dest, NavMeshAgent agent)
    {
        Debug.Log($"AgentAttack is about to be set!");
        SetOpponentMode(OpponentMode.Attacking);
        opponentUtils.WalkTowardCoordinates(dest, agent);

    }


    public void Wander(Vector3 dest, NavMeshAgent agent)
    {
        SetOpponentMode(OpponentMode.Exploring);
        opponentUtils.WalkTowardCoordinates(dest, agent);
    }

    public void WalkFollowMouseClick(NavMeshAgent agent)
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            agent.destination = opponentUtils.GetMouseClickVector();
            SetOpponentMode(OpponentMode.Exploring);
        }
    }


        public void SetOpponentMode(OpponentMode mode)
    {   
        opponentMode = mode;
        switch(mode)
        {
            case OpponentMode.Exploring:
            {
                speed = walkingSpeed;
                break;
            }
            case OpponentMode.Attacking:
            {
                speed = runningSpeed;
                break;
            }

        }
        agent.speed = speed;
    }
    public OpponentMode GetOpponentMode()
    {
        return opponentMode;
    }



}
