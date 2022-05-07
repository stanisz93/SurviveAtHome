using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;


public enum OpponentMode {Exploring, Rushing, HitPlayer, Scream, Agonize, Fall, beingKicked, Attacking, Checking, LookAround, Smelling};
public class OpponentActions : MonoBehaviour
{
    public OpponentEventController opponentEventController;
    public LayerMask PushStopperMask;
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

    public Animator animator;
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
    private Opponent opponent;
    private Rigidbody m_Rigidbody;

    public Transform playerTemp;
    

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
        opponent = GetComponent<Opponent>();
        m_Rigidbody = GetComponent<Rigidbody>();
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

    public IEnumerator Fall()
    {
        opponentEventController.StartTask();
        SetOpponentMode(OpponentMode.Fall);
        animator.SetTrigger("Fall");
        transform.DOMove(m_Rigidbody.transform.position + m_Rigidbody.transform.forward * 2f, .5f);
        vfov.ResetSense(7f);
        yield return new WaitForSeconds(3f);
        animator.SetTrigger("StandAfterFall");
        while(opponentEventController.TaskIsBlocked())
        {
            yield return null;
        }
        taskManager.TaskSetToFinish();    
    }


    public IEnumerator GotPushed(Transform player, float pushForce, float pushTime)
    {
        opponentEventController.StartTask();
        SetOpponentMode(OpponentMode.beingKicked);
        Vector3 targetDir = transform.position - player.position;
        Vector3 targetDirNorm = new Vector3(targetDir.x, transform.position.y, targetDir.z).normalized;
        float playerVelocity = player.GetComponentInParent<Character>().SpeedBeforeKick;
        if(playerVelocity > 5f)
        {
            if(pushForce < 2f)
            {
                pushForce *= 2;
                pushTime *= 2;
            }
        }
        Vector3 pushVect = new Vector3(pushForce * targetDirNorm.x, 0f, pushForce * targetDirNorm.z);
        // Vector3 destPos = transform.position - targetDirection.normalized * pushForce;

        //Estimate max distance
        RaycastHit hit;
        bool hitTheObstacle = false;
        if (Physics.Raycast(transform.position, targetDirNorm, out hit, 1.1f * pushForce, PushStopperMask))
            {
                pushVect = hit.point - transform.position;
                hitTheObstacle = true;
            }
        Vector3 destPos = transform.position + 0.8f * pushVect;
        
        Sequence pushS = DOTween.Sequence();
        pushS.Append(transform.DOMove(destPos, pushTime));
        if(hitTheObstacle)
            {
             pushS.AppendCallback(() => animator.SetTrigger("HitObstacleWhilePushed"));
             pushS.Append(transform.DOMove(transform.position + 0.7f * pushVect, 0.1f));
            }
        transform.rotation = Quaternion.LookRotation(-player.forward);


        animator.SetFloat("PlayerSpeedKick", playerVelocity);
        animator.SetTrigger("beingKicked");
        // vfov.ResetSense(2f);
        while(opponentEventController.TaskIsBlocked())
        {
            yield return null;
        }
        taskManager.TaskSetToFinish();    
    }

    public IEnumerator AttackSequenceTask(Transform player, int damage)
    {
        yield return NoticePlayer(player);
        yield return RunTowardPlayer(player);
        yield return HitPlayer(player, damage);
        yield return new WaitForSeconds(damageInterval);
        taskManager.TaskSetToFinish();

    }
    
    public IEnumerator RunTowardPlayer(Transform player)
    {
        if (!ReachPlayerRange(player.position))
            SetOpponentMode(OpponentMode.Rushing);
        else
        {
            SetOpponentMode(OpponentMode.Exploring);
        } // here reaction of seeng player is runned
        while(!ReachPlayerRange(player.position) && vfov.FoundedObject())
        {
            SetLastPlayerPosition(player);
            agent.destination = player.position;
            yield return new WaitForSeconds(changeRushingDecision);
        }

    }

    public IEnumerator NoticePlayer(Transform player)
    {
        if(GetOpponentMode() != OpponentMode.Scream && !nextAttack)
        {   
            SetOpponentMode(OpponentMode.Scream);
            yield return RotateTowardPosUntil(player, 2f);
        }
    }

    IEnumerator HitPlayer(Transform player, int damage)
    {
        var plr = player.gameObject.GetComponent<Character>();
        if (plr.justDied())
        {
            vfov.ResetSense();
            SetOpponentMode(OpponentMode.Exploring);
            nextAttack = false;
            yield break;
        }
        else if(vfov.FoundedObject())
        {   
            yield return RotateTowardPlayer(player);
            nextAttack = true;
            if(ReachPlayerRange(player.position))
            {
                SetOpponentMode(OpponentMode.HitPlayer);
                plr.ReduceHealth(damage); 
            }
        }
        else
        {
            nextAttack = false;
        }
            


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
        float timePassed = 0f;
        while(timePassed < time)
        {
            Vector3 targetDirection = pos.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }


    public IEnumerator Exploring()
    {
        if(Random.value >= 0.6)
            yield return Idle();
        else
        {
            Vector3 randomDest = opponentUtils.FindRandomDestination();  
            yield return WalkTowardCoordinates(randomDest);
        }
        
    }

    public IEnumerator Idle()
    {
        SetOpponentMode(OpponentMode.Agonize);
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
        yield return WalkTowardCoordinates(playerSeen, 0.5f, false, false, false);
        SetOpponentMode(OpponentMode.LookAround);
        yield return new WaitForSeconds(4f);
        taskManager.TaskSetToFinish();
    }

    IEnumerator WalkTowardCoordinates(Vector3 coordinates, float distErr = 0f, bool justExploring=true, bool coolOff=true, bool finishTask=true)
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
        if(finishTask)
            taskManager.TaskSetToFinish();
}


        public void SetOpponentMode(OpponentMode mode)
    {   
        opponentMode = mode;
        switch(mode)
        {
            
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
            default:
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


void Update()
 {
     //DEBUG DESTINATION PUSH POINT
        // Vector3 targetDirection = transform.position - playerTemp.position;
        // targetDirection = new Vector3(targetDirection.x, transform.position.y, targetDirection.z).normalized;
        // var pushVect = new Vector3(5f * targetDirection.x, 0f, 5f * targetDirection.z);
        // RaycastHit hit;
        // if (Physics.Raycast(transform.position, targetDirection, out hit, 5f, PushStopperMask))
        // {
        //     var XYPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        //     // var TowardWall
        //     pushVect = hit.point - transform.position;
        //     // pushVect = new Vector3(pushVect.x * targetDirection.x, 0f, pushVect.z * targetDirection.z);
        //     ShapeUtils.DrawLine(transform.position,  hit.point, Color.white);
        // }
        // Vector3 targetPoint = transform.position + pushVect;
        // ShapeUtils.DrawLine(transform.position,  targetPoint, Color.red);
    }   
}
