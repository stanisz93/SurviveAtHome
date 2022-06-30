using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
[RequireComponent(typeof(Inventory))]
public class Character : MonoBehaviour
{
    public Transform DebugBall;
    public EnergyConsumption energyConsumption;
    private CharacterMovement characterMovement;
    public Camera mainCamera;
    public Transform respawn;
    public LayerMask terrainMask;
    public LayerMask opponentSnapMask;
    public Camera DeathCamera;
    public HealthBar healthBar;
    public TriggerAction triggeredAction;
    public float SpeedBeforeKick;
    public Transform leftHand;
    public GameObject pfBloodEffect;
    public Transform bloodEffectPos;

    public Transform head;
    public Transform opponentFocus = null;
    private Health health;
    public Animator animator;
    private PlayerTriggers playerTriggers;
    private Opponent latestAttacker;

    public delegate void TriggerAction();

    public delegate IEnumerator OnConditionSatisfiedToInterrupt();

    private TriggerAction LastAllowInterruption;

    private Inventory Inventory;

    private PlayerController playerController;
    private HitBonus bonus;
    private Endurance endurance;

    private ObstacleInteractionManager obstManager;
    private Coroutine interruptionCoroutine;

    private Vector3 targetDirection;
    private OpponentMagnet opponentMagnet;

    private bool isFightModeOn = false;
    private Coroutine fightCoroutine;



    

    // Start is called before the first frame update

    // Update is called once per frame

    void Awake() {
        DeathCamera.enabled = false;
        mainCamera.enabled = true;
        health = GetComponent<Health>();
        characterMovement = GetComponent<CharacterMovement>();
        playerTriggers = GetComponent<PlayerTriggers>();
        Inventory = GetComponent<Inventory>();
        endurance = GetComponent<Endurance>();
        ResetPlayer(); 
        bonus = GetComponent<HitBonus>();
        health.OnDamageTake += healthBar.ReduceValue;
        health.OnDamageTake += TakeDamageEffect;
        health.OnDie += playerTriggers.Die;
        playerController = GetComponent<PlayerController>();
        obstManager = GetComponent<ObstacleInteractionManager>();
        opponentMagnet = GetComponentInChildren<OpponentMagnet>();
        }

    public void ResetVelocity() => characterMovement.ResetVelocity();

    public Transform GetCurrentObstacle() => obstManager.GetCurrentObstacle();

    public bool IsFightMode() => isFightModeOn;

    public void ResetPlayer()
    {
        transform.position = respawn.position;
        healthBar.SetMaxHealth(health.maxHealth);
 
    }

    public Vector3 GetHeadPosition()
    {
        return head.position;
    }

    public bool justDied()
    {
        return playerTriggers.dying;
    }


void SetToFightMode()
{
    isFightModeOn = true;
    animator.SetFloat("FightMode", 1f);
}

public void SetToDefaultMovement()
    {
        isFightModeOn = false;
        animator.SetFloat("FightMode", 0f);
    }


    IEnumerator TurnOnInteruptionAfter(TriggerAction delayedFun, OnConditionSatisfiedToInterrupt satisfier)
    //very specific function used to allow for interruption but after specific time

    //This is tricky, there might be case where TriggerAction with allowed interruption
    //passed, but hasn't been later released, then interruption have managed
    //to get into satisfier loop, and when another one Trigger with allowed interruption is 
    //run then this run is still waiting and can be triggered, which it should be canceled.
    ///PL chodzi o to ze StartTriggerAction moze nie dosc do skutku, ale 
    //jako ze odpalila sie metoda StartTriggerAction, to allowed metoda
    //może wejść w czekanie na satisfier, i wtedy przy kolejnym StartTrigger z suckese.
    //wejdzie i się zrealuzuje, a nie powinno. Powinno moc sie zrealizowac
    //tylko Interruptor ktory wystapil po Funkcji Trigerującej
    {
        yield return satisfier();
        LastAllowInterruption = delayedFun;
    }
    

    public void StartTriggerAction(TriggerAction triggerAction, TriggerAction allowed=null, OnConditionSatisfiedToInterrupt satisfier=null)
    {
            if(interruptionCoroutine != null)
                StopCoroutine(interruptionCoroutine); //This stopping need to be applied, otherwise previous coroutines
                ///with staisfied satisfier(TurnOnInteruptionAfter)  will be runned
            if(playerTriggers.isTriggerEmpty || LastAllowInterruption == triggerAction)
            {    
                    triggeredAction = triggerAction;
                    if(LastAllowInterruption == triggerAction)
                        LastAllowInterruption = null;
            }


            if(allowed != null && satisfier != null)
                {
                    interruptionCoroutine = StartCoroutine(TurnOnInteruptionAfter(allowed, satisfier));
                }



    }

    public IEnumerator KeepLatestAttacker(Opponent opponent)
    {
        float timer = 0f;
        latestAttacker = opponent;
        Opponent currentAttacker = opponent;
        while(timer < 3f)
        {
            timer += Time.deltaTime;
            if(latestAttacker == currentAttacker)
                yield return null;
            else
                break;
        }
        if(latestAttacker == currentAttacker)
            latestAttacker = null;

    }

    // public Opponent GetLatestAttacker()
    // {
    //     return latestAttacker;
    // }


    private void Update() {
        // ShapeUtils.DrawLine(characterMovement.t_mesh.transform.position, characterMovement.t_mesh.transform.position + characterMovement.t_mesh.forward, Color.red);
        var currPos = characterMovement.t_mesh.transform.position;
        // ShapeUtils.DrawLine(currPos, currPos + characterMovement.t_mesh.transform.forward, Color.red);
        // ShapeUtils.DrawLine(currPos, currPos + characterMovement.Velocity, Color.yellow);
        
    }

    public Vector3 GetVectorRelativeToCamera(float forward, float right)
    {
        Vector3 CamFwd = mainCamera.transform.forward;
        Vector3 CamRight = mainCamera.transform.right;
        Vector3 translation = forward * mainCamera.transform.forward;
        translation += right * mainCamera.transform.right;
        translation.y = 0f;
        Vector3 _velocity = Vector3.zero;
        if (translation.magnitude > 0)
        {
            _velocity = translation;
        }

        return _velocity.normalized;
    }

    public void ToogleKinematic()
    {
        characterMovement.ToggleRigidbody();
    }

    public void BlockPlayerControl()
    {
        playerTriggers.BlockTrigger();
        playerTriggers.BlockMovement();
    }


    public IEnumerator BlockPlayerControlForSeconds(float delay)
    {
        BlockPlayerControl();
        yield return new WaitForSeconds(delay);
        playerTriggers.ReleasePlayerControl();
    }

    IEnumerator FightModeForSeconds(float delay)
    {
            SetToFightMode();
            yield return new WaitForSeconds(delay);
            SetToDefaultMovement();
    }

    public void TryToRunFightMode()
    {
        if(fightCoroutine != null)
            StopCoroutine(fightCoroutine);
        fightCoroutine = StartCoroutine(FightModeForSeconds(5f));
    }

    public void TakeDamageEffect(int damage)
    {
        TryToRunFightMode();
        if(bonus.hitCounts > 0)
            bonus.ResetCounts();
        var bloodEffect = Instantiate(pfBloodEffect, bloodEffectPos.position, bloodEffectPos.rotation);
        bloodEffect.GetComponent<ParticleSystem>().Play();
    }
    

    public void AddMovementInput(float forward, float right)
    {
        Vector3 movementRelativeToCamera = GetVectorRelativeToCamera(forward, right);

        characterMovement.Velocity = movementRelativeToCamera;
    }

    public void AddTargetInput(float forward, float right)
    {
        targetDirection = GetVectorRelativeToCamera(forward, right);
    }

    // void RotateTowardSnappedOpponent()
    // {
    //     var relDir = transform.position - opponentFocus.position;
    //     relDir = - new Vector3(relDir.x, 0f, relDir.z);

    //     Quaternion wantedRotation = Quaternion.LookRotation(relDir, Vector3.up);
    //     characterMovement.t_mesh.rotation = Quaternion.Slerp(characterMovement.t_mesh.rotation, wantedRotation, Time.deltaTime * 20);
    //     // characterMovement.t_mesh.LookAt(goal);
    //     DebugBall.position = new Vector3(opponentFocus.transform.position.x, DebugBall.position.y, opponentFocus.transform.position.z);
    
    // }

    public Vector3 GetForward()
    {
        return characterMovement.t_mesh.forward;
    }


    public Vector3 GetTargetInput() => targetDirection;

    public float GetVelocityMagnitude()
    {
        // Debug.Log(characterMovement.Velocity.magnitude);
        return characterMovement.Velocity.magnitude;
    }

    public Vector3 GetForwardDirection()
    {
        return characterMovement.t_mesh.forward;
    }

    public Transform GetCharacterMeshTransform() => characterMovement.t_mesh;

    public Vector3 GetVelocityDirection()
    {
        return characterMovement.Velocity.normalized;
    }

    public Vector3 GetDirectionWithRespectToOpponent() 
    {
        if(opponentMagnet.NearestOpponent != null)
            // {
            //     Vector3 translation = forward * mainCamera.transform.forward;
            //     translation += right * mainCamera.transform.right;
            //     translation.y = 0f;
            return Vector3.Scale(opponentMagnet.DirectionToOpponent(), characterMovement.Velocity.normalized);
            // }
            // return characterMovement.Velocity.normalized;//opponentMagnet.DirectionToOpponent() - characterMovement.Velocity.normalized;
        else
            return characterMovement.Velocity.normalized;
    }

    public FightMode GetFightMode(){return characterMovement.GetFightMode();}

    public MovementMode GetMovement(){return characterMovement.GetMovementMode();}

    public void SetFightmode(FightMode mode){characterMovement.SetFightMode(mode); }

    public void SetToRun(){
        SetToDefaultMovement();
        characterMovement.SetMovementMode(MovementMode.Running);}

    public void ToggleCrouch(){
        if(characterMovement.GetMovementMode() == MovementMode.Crouching)
            SetToWalk();
        else
            characterMovement.SetMovementMode(MovementMode.Crouching);
        }
    public void SetToWalk(){characterMovement.SetMovementMode(MovementMode.Fight);}

}
