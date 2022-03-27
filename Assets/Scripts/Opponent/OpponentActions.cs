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
    public float stoppingDistance = 0.0f;
    OpponentMode opponentMode = OpponentMode.Exploring;
    private OpponentUtils opponentUtils;
    private NavMeshAgent agent;
    private VisionFieldOfView vfov;
    private Opponent opponent;
    private Coroutine startedCoroutine;
    
    void Start() 
    {
        opponent = GetComponent<Opponent>();
        agent = GetComponent<NavMeshAgent>();
        opponentUtils = GetComponent<OpponentUtils>();
        vfov = GetComponentInChildren<VisionFieldOfView>();
        StartCoroutine(ListenToInterruptions());
        
    }
    IEnumerator ListenToInterruptions()
    {
        while(true)
        {
            if (vfov.GetPlayerTarget() != null && !opponent.IsDuringTask())
            {
                if(startedCoroutine!=null)
                    StopCoroutine(startedCoroutine);
                startedCoroutine = StartCoroutine("AgentAttack", vfov.GetPlayerTarget());
            }
            else if(Input.GetMouseButtonDown(0))
            {
                if(startedCoroutine!=null)
                    StopCoroutine(startedCoroutine);
                startedCoroutine = StartCoroutine("WalkFollowMousePosition");
            }
            yield return null;

        }



    }

    // Update is called once per frame

    private bool ReachPlayerRange(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < agent.stoppingDistance;
    }
    IEnumerator AgentAttack(Transform player)
    {
        opponent.StartTask();
        opponent.SetInterruption(true);
        SetOpponentMode(OpponentMode.Rushing);
        if(GameSystem.Instance.opponentDebug) Debug.Log($"Agent is trying to reach player!");
        while(!ReachPlayerRange(player.position))
        {
            agent.destination = player.position;
            yield return new WaitForSeconds(opponent.changeRushingDecision);
        }
    
        yield return StartCoroutine("HitUntilDead", player);
        opponent.SetInterruption(false);
        opponent.FinishTask();

    }
    IEnumerator HitUntilDead(Transform player)
    {
        SetOpponentMode(OpponentMode.Attacking);
        var plr = player.gameObject.GetComponent<Character>();
        while(!opponent.IsInterrupted() && Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
        {
            plr.ReduceHealth(opponent.damage);
            int currentHealth = plr.GetHealth();
            if(GameSystem.Instance.opponentDebug) Debug.Log($"Opponent takes {opponent.damage} damage, player has {currentHealth} health.");
            yield return null;
            // yield return new WaitForSeconds(opponent.damageSpeed);
        }
    }


    public IEnumerator Exploring()
    {
        Vector3 randomDest = opponentUtils.FindRandomDestination();  
        yield return StartCoroutine("WalkTowardCoordinates", randomDest);
        
    }
    
    IEnumerator WalkFollowMousePosition()
    {       Vector3 mousePosition = opponentUtils.GetMousePosition();
            opponent.StartTask();
            opponent.SetInterruption(true);
            yield return StartCoroutine("WalkTowardCoordinates", mousePosition);
            opponent.FinishTask();
            opponent.SetInterruption(false);
    }

    IEnumerator WalkTowardCoordinates(Vector3 coordinats)
    {
            opponentUtils.WalkTowardCoordinates(coordinats, agent);
            SetOpponentMode(OpponentMode.Exploring);
            opponent.SetInterruption(false);
            while(agent.remainingDistance != 0)
            {   
                if (opponent.IsInterrupted())
                    {
                        yield break;
                    }
                yield return new WaitForSeconds(.2f);
            }
            if(GameSystem.Instance.opponentDebug) Debug.Log($"Agent destination: {agent.destination}");
    }



        public void SetOpponentMode(OpponentMode mode)
    {   
        opponentMode = mode;
        switch(mode)
        {
            case OpponentMode.Exploring:
            {
                speed = walkingSpeed;
                stoppingDistance = 0.0f;
                break;
            }
            case OpponentMode.Rushing:
            {
                speed = runningSpeed;
                stoppingDistance = 1.2f;
                break;
            }
            case OpponentMode.Attacking:
            {
                speed = 0f;
                break;
            }

        }
        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;
    }
    public OpponentMode GetOpponentMode()
    {
        return opponentMode;
    }



}
