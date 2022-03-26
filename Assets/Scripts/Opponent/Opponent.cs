using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Opponent : MonoBehaviour
{
    private delegate void Task(Vector3 dest, NavMeshAgent agent);

    public bool RandomExplore = true;
    public bool debug = false;
    private OpponentActions opponentActions;
    private NavMeshAgent agent;
    // private Task performTask;
    private OpponentUtils opponentUtils;

    // Start is called before the first frame update
    void Start()
    {   
        agent = GetComponent<NavMeshAgent>();
        opponentActions = GetComponent<OpponentActions>();
        opponentUtils = GetComponent<OpponentUtils>();
    }



    void TryToSetTask(Task task, Vector3 coords)
    {
        if(AgentIsFree())
        {
            task(coords, agent);
        }
    }

    bool AgentIsFree()
    {
        return agent.remainingDistance == 0.0 ? true : false;
    }

    private void ForceTask(Task task, Vector3 coords)
    {
        if(debug)
        Debug.Log($"Forced task {task} to an agent {agent}");
        task(coords, agent);

    }

    public void Attack(Vector3 player)
    {   
        if(debug)
        Debug.Log("Try ATTACK!");
        ForceTask(opponentActions.AgentAttack, player);
    }

    // Update is called once per frame
    void Update() 
    {
        
        if (RandomExplore)
        {   
            if(AgentIsFree())
                {   
                    if (debug) Debug.Log("Agent is free, adding task"); 
                    Vector3 randomDest = opponentUtils.FindRandomDestination();
                    ForceTask(opponentActions.Wander, randomDest);
                }
        }
        else
        {
            opponentActions.WalkFollowMouseClick(agent);
        }
    }
}

