using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum OpponentMode {Exploring, Rushing, Attacking, Smelling};

public enum ActionType {AllowToStop, ForbiddenStop, None}
public class OpponentActions : MonoBehaviour
{
    public float walkingSpeed = 0.5f;
    public float runningSpeed = 3.0f;
    private float speed = 1.5f;
    public float attackMinDist = 1f;
    public float damageInterval = 0.4f;
    public float reactionDelay = .4f;
    public float rotationSpeed = 10f;
    public float changeRushingDecision = .05f;

    private float stoppingDistance = 0.0f;
    
    OpponentMode opponentMode = OpponentMode.Exploring;
    private OpponentUtils opponentUtils;
    private NavMeshAgent agent;
    private VisionFieldOfView vfov;
    private Coroutine startedCoroutine;
    private TaskManager taskManager;
    public ActionType currentAction;
    private bool alerted = false;
    private Vector3 playerSeen;
    
    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        opponentUtils = GetComponent<OpponentUtils>();
        vfov = GetComponentInChildren<VisionFieldOfView>();
        taskManager = GetComponent<TaskManager>();
        
    }


    // Update is called once per frame

    private bool ReachPlayerRange(Vector3 position, float deltaDist = 0.0f)
    {
        return Vector3.Distance(transform.position, position) <= (attackMinDist + deltaDist);
    }

    public bool isAlerted() {return alerted;}

    void SetLastPlayerPosition(Transform player) { playerSeen = player.position;}

    public IEnumerator AgentAttack(Transform player, int damage)
    {
        currentAction = ActionType.ForbiddenStop;
        agent.destination = transform.position;
        SetOpponentMode(OpponentMode.Rushing);
        // yield return new WaitForSeconds(2f);
        yield return RotateTowardPosUntil(player, 2f);
        if(GameSystem.Instance.opponentDebug) Debug.Log($"Agent is trying to reach player!");
        while(!ReachPlayerRange(player.position) && vfov.FoundedObject())
        {
            agent.destination = player.position;
            yield return new WaitForSeconds(changeRushingDecision);
        }
        // yield return new WaitForSeconds(0.05f);
        agent.destination = transform.position;
        yield return HitUntilDead(player, damage);
        taskManager.TaskSetToFinish();
    }

    public IEnumerator RotateTowardPlayer()
    {       
            // agent.updateRotation = false;
            while(Vector3.Angle(transform.forward, (playerSeen - transform.position).normalized) > 5)
            {
                Vector3 targetDirection = playerSeen - transform.position;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(newDirection);
                yield return null;
            }
            // agent.updateRotation = true;
    }

    public IEnumerator RotateTowardPosUntil(Transform pos, float time)
    {       
        float timePassed = 0;
        while(timePassed < time)
        {
            Vector3 targetDirection = pos.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }
 

    IEnumerator HitUntilDead(Transform player, int damage)
    {
        currentAction = ActionType.ForbiddenStop;
        var plr = player.gameObject.GetComponent<Character>();
        SetOpponentMode(OpponentMode.Attacking);
        // Coroutine rotateCoroutine = StartCoroutine(RotateToPlayer(player));
        while(ReachPlayerRange(player.position, 0.05f))
        {
            plr.ReduceHealth(damage);
            yield return new WaitForSeconds(damageInterval);
            SetLastPlayerPosition(player);
            yield return RotateTowardPlayer();
        }

    }


    public IEnumerator Exploring()
    {
        currentAction = ActionType.AllowToStop;
        Vector3 randomDest = opponentUtils.FindRandomDestination();  
        return WalkTowardCoordinates(randomDest);
        
    }
    
    public IEnumerator WalkFollowMousePosition()
    {   
        currentAction = ActionType.AllowToStop;
        Vector3 mousePosition = opponentUtils.GetMousePosition();
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
        SetCurrentActionToNone();
}

    private void SetCurrentActionToNone()
    {
        currentAction = ActionType.None;
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
