using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Opponent : MonoBehaviour
{
    public bool debug = false;
    public float reactionDelay = .4f;
    public float changeRushingDecision = .1f;
    public float damageSpeed = 1f;
    public int damage = 20;
    private OpponentActions opponentActions;
    private NavMeshAgent agent;
    // private Task performTask;
    private OpponentUtils opponentUtils;
    private bool duringTask = false;
    private bool interrupted = false;
    

    // Start is called before the first frame update
    void Start()
    {   
        agent = GetComponent<NavMeshAgent>();
        opponentActions = GetComponent<OpponentActions>();
        opponentUtils = GetComponent<OpponentUtils>();
        StartCoroutine("ActivateOpponent");

    }

    public void StartTask()
    {
        duringTask = true;
    }
    public void FinishTask()
    {
        duringTask = false;
    }
    public bool IsDuringTask(){return duringTask;}

    public void SetInterruption(bool isInterrupted)
    {
        interrupted = isInterrupted;
    }
    public bool IsInterrupted() {return interrupted;}


    // Update is called once per frame
    public IEnumerator ActivateOpponent() 
    {
        while(true)
        {
            while(duringTask)
                yield return null;
            yield return new WaitForSeconds(reactionDelay);
            yield return StartCoroutine(opponentActions.Exploring());
            }
    }

}

