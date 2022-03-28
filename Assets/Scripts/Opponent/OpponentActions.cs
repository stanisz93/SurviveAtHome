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
    public float attackMinDist = 1.2f;

    public float reactionDelay = .4f;
    public float changeRushingDecision = .1f;
    OpponentMode opponentMode = OpponentMode.Exploring;
    private OpponentUtils opponentUtils;
    private NavMeshAgent agent;
    private VisionFieldOfView vfov;
    private Coroutine startedCoroutine;
    private TaskManager taskManager;
    
    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        opponentUtils = GetComponent<OpponentUtils>();
        vfov = GetComponentInChildren<VisionFieldOfView>();
        taskManager = GetComponent<TaskManager>();
        
    }


    // Update is called once per frame

    private bool ReachPlayerRange(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) < attackMinDist;
    }


    public IEnumerator AgentAttack(Transform player, int damage)
    {
        SetOpponentMode(OpponentMode.Rushing);
        if(GameSystem.Instance.opponentDebug) Debug.Log($"Agent is trying to reach player!");
        while(!ReachPlayerRange(player.position))
        {
            agent.destination = player.position;
            yield return new WaitForSeconds(changeRushingDecision);
        }
    
        yield return StartCoroutine(HitUntilDead(player, damage));

    }
    IEnumerator HitUntilDead(Transform player, int damage)
    {
        SetOpponentMode(OpponentMode.Attacking);
        var plr = player.gameObject.GetComponent<Character>();
        while(ReachPlayerRange(player.position))
        {
            plr.ReduceHealth(damage);
            int currentHealth = plr.GetHealth();
            if(GameSystem.Instance.opponentDebug) Debug.Log($"Opponent takes {damage} damage, player has {currentHealth} health.");
            yield return null;
        }
        taskManager.TaskSetToFinish();

    }


    public IEnumerator Exploring()
    {
        Vector3 randomDest = opponentUtils.FindRandomDestination();  
        return WalkTowardCoordinates(randomDest);
        
    }
    
    public IEnumerator WalkFollowMousePosition()
    {       Vector3 mousePosition = opponentUtils.GetMousePosition();
            yield return WalkTowardCoordinates(mousePosition);
    }

    IEnumerator WalkTowardCoordinates(Vector3 coordinats)
    {
        // This could be expanded in more interesting way of walking,
        // Brown motion, or at least more smooth that is now using Slerp and
        // turning off agent.updatePositon
            opponentUtils.WalkTowardCoordinates(coordinats, agent);
            if(GameSystem.Instance.opponentDebug) Debug.Log($"Agent destination: {agent.destination}");
    
            SetOpponentMode(OpponentMode.Exploring);
            while(agent.remainingDistance > 0)
            {   
                yield return null;
            }
                yield return new WaitForSeconds(reactionDelay);
        taskManager.TaskSetToFinish();
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
                stoppingDistance = attackMinDist;
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
