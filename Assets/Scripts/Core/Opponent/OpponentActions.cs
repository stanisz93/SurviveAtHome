using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum OpponentMode {Exploring, Rushing, Idle, Attacking, Checking, Smelling};
public class OpponentActions : MonoBehaviour
{
    public float walkingSpeed = 0.3f;
    public float runningSpeed = 3.0f;
    public float checkingSpeed = 1.5f;
    private float speed = 1.5f;
    public float attackMinDist = 1f;
    public float damageInterval = 0.4f;
    public float ExploringInterval = 1f;
    public float rotationSpeed = 10f;
    public float changeRushingDecision = .05f;

    public Transform playerSeenHelper;
    private float stoppingDistance = 0.0f;
    
    OpponentMode opponentMode = OpponentMode.Exploring;
    private OpponentUtils opponentUtils;
    private NavMeshAgent agent;
    private VisionFieldOfView vfov;
    private Coroutine startedCoroutine;
    private TaskManager taskManager;
    private bool alerted = false;
    private bool nextAttack = false; //this bool mean if this is next attempt after first attack
    private Vector3 playerSeen;
    

     private void Awake () 
     {
        playerSeenHelper.SetParent(null, true);
    }

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

    void SetLastPlayerPosition(Transform player)
    { 
        playerSeen = player.position;
        playerSeenHelper.position = playerSeen;
    }

    public IEnumerator AgentAttack(Transform player, int damage)
    {
        agent.destination = transform.position;
        SetOpponentMode(OpponentMode.Rushing);
        // yield return new WaitForSeconds(2f);
        if (!nextAttack)
            yield return RotateTowardPosUntil(player, 2f); // here reaction of seeng player is runned
        if(GameSystem.Instance.opponentDebug) Debug.Log($"Agent is trying to reach player!");
        while(!ReachPlayerRange(player.position) && vfov.FoundedObject())
        {
            SetLastPlayerPosition(player);
            agent.destination = player.position;
            yield return new WaitForSeconds(changeRushingDecision);
        }
        if(vfov.FoundedObject())
        {
            yield return HitUntilDead(player, damage);
        }
        else
        {
            nextAttack = false;
        }
        taskManager.TaskSetToFinish();
    }

    public IEnumerator RotateTowardPlayer(Transform player)
    {       
            // agent.updateRotation = false;
            while(Vector3.Angle(transform.forward, (player.position - transform.position).normalized) > 5)
            {
                Vector3 targetDirection = player.position - transform.position;
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
         agent.destination = transform.position;
        var plr = player.gameObject.GetComponent<Character>();
        SetOpponentMode(OpponentMode.Attacking);
        // Coroutine rotateCoroutine = StartCoroutine(RotateToPlayer(player));
        while(ReachPlayerRange(player.position, 0.05f))
        {
            SetLastPlayerPosition(player);
            plr.ReduceHealth(damage);
            yield return new WaitForSeconds(damageInterval);
            yield return RotateTowardPlayer(player);

        }
        nextAttack = true;

    }


    public IEnumerator Exploring()
    {
        Vector3 randomDest = opponentUtils.FindRandomDestination();  
        return WalkTowardCoordinates(randomDest);
        
    }

    public IEnumerator Idle()
    {
        SetOpponentMode(OpponentMode.Idle);
        yield return new WaitForSeconds(4f);
        taskManager.TaskSetToFinish();
    }
    
    public IEnumerator WalkFollowMousePosition()
    {   
        Vector3 mousePosition = opponentUtils.GetMousePosition();
        yield return WalkTowardCoordinates(mousePosition, 0f, true, false);
    }

    public IEnumerator CheckSuspiciousPlace()
    {
        SetOpponentMode(OpponentMode.Checking);
        yield return WalkTowardCoordinates(playerSeen, 0.5f, false, false);
    }

    IEnumerator WalkTowardCoordinates(Vector3 coordinates, float distErr = 0f, bool justExploring=true, bool coolOff=true)
    {
        // This could be expanded in more interesting way of walking,
        // Brown motion, or at least more smooth that is now using Slerp and
        // turning off agent.updatePositon
        
        if(justExploring)
            SetOpponentMode(OpponentMode.Exploring);
        if(coolOff)
            
            {
                yield return new WaitForSeconds(ExploringInterval);
                agent.destination = transform.position;
            }
            
        opponentUtils.WalkTowardCoordinates(coordinates, agent);
        if(GameSystem.Instance.opponentDebug) Debug.Log($"Agent destination: {agent.destination}");

        while(agent.remainingDistance > distErr)
        {   
            yield return null;
        }
        taskManager.TaskSetToFinish();
}


        public void SetOpponentMode(OpponentMode mode)
    {   
        opponentMode = mode;
        switch(mode)
        {
            case OpponentMode.Idle:
            {
                speed = 0f;
                break;
            }

            case OpponentMode.Exploring:
            {
                speed =  walkingSpeed;
                stoppingDistance = 0.0f;
                break;
            }
            case OpponentMode.Rushing:
            {
                speed = runningSpeed;
                stoppingDistance = attackMinDist;
                break;
            }
            case OpponentMode.Checking:
            {
                speed = checkingSpeed;
                stoppingDistance = 0.0f;
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
