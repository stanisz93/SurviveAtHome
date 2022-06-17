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

    private PlayerTriggers playerTriggers;

    public delegate void TriggerAction();

    public delegate IEnumerator OnConditionSatisfiedToInterrupt();

    private TriggerAction LastAllowInterruption;

    private Inventory Inventory;

    private Vector3 currrentMouseDirection {get => MouseUtils.MousePositon(mainCamera, characterMovement.t_mesh, terrainMask);}

    private HitBonus bonus;
    private Endurance endurance;

    private Coroutine interruptionCoroutine;

    

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
        }


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


    private void Update() {
        // ShapeUtils.DrawLine(characterMovement.t_mesh.transform.position, characterMovement.t_mesh.transform.position + characterMovement.t_mesh.forward, Color.red);
        var currPos = characterMovement.t_mesh.transform.position;
        ShapeUtils.DrawLine(currPos, currPos + characterMovement.t_mesh.transform.forward, Color.red);
        ShapeUtils.DrawLine(currPos, currPos + characterMovement.Velocity, Color.yellow);
        
    }

    private Vector3 GetVectorRelativeToCamera(float forward, float right)
    {
        Vector3 CamFwd = mainCamera.transform.forward;
        Vector3 CamRight = mainCamera.transform.right;
        Vector3 translation = forward * mainCamera.transform.forward;
        translation += right * mainCamera.transform.right;
        translation.y = 0f;
        Vector3 velocity = Vector3.zero;
        if (translation.magnitude > 0)
        {
            velocity = translation;
        }

        return velocity.normalized;
    }



    public void TakeDamageEffect(int damage)
    {
        SetFightmode(FightMode.ReceiveDamage);
        if(bonus.hitCounts > 0)
            bonus.ResetCounts();
        var bloodEffect = Instantiate(pfBloodEffect, bloodEffectPos.position, bloodEffectPos.rotation);
        bloodEffect.GetComponent<ParticleSystem>().Play();
    }
    

    public void AddMovementInput(float forward, float right)
    {
        var mouseMovement = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));
        if(Input.GetMouseButton(1))
        //maybe worth to use here Slerp instead of sudden rotation
            SnapTowardOpponent();
        // var localVelocity = GetVectorRelativeToCamera(forward, right);
        if(opponentFocus != null)
            RotateTowardSnappedOpponent();
        Vector3 movementRelativeToCamera = GetVectorRelativeToCamera(forward, right);

        characterMovement.Velocity = movementRelativeToCamera;
    }

    void RotateTowardSnappedOpponent()
    {
        var relDir = transform.position - opponentFocus.position;
        relDir = - new Vector3(relDir.x, 0f, relDir.z);

        Quaternion wantedRotation = Quaternion.LookRotation(relDir, Vector3.up);
        characterMovement.t_mesh.rotation = Quaternion.Slerp(characterMovement.t_mesh.rotation, wantedRotation, Time.deltaTime * 20);
        // characterMovement.t_mesh.LookAt(goal);
        DebugBall.position = new Vector3(opponentFocus.transform.position.x, DebugBall.position.y, opponentFocus.transform.position.z);
    
    }

    void SnapTowardOpponent()
    {
            RaycastHit hit;
            var headPosition = transform.position;
            headPosition = new Vector3(headPosition.x, 0.5f, headPosition.z);
            if (Physics.Raycast(headPosition, currrentMouseDirection, out hit, 5f, opponentSnapMask))
            {
                DebugBall.gameObject.SetActive(true);
                opponentFocus = hit.transform;
                DebugBall.position = new Vector3(opponentFocus.transform.position.x, DebugBall.position.y, opponentFocus.transform.position.z);
                DebugBall.parent = opponentFocus;
            }
            else
            {
                DebugBall.gameObject.SetActive(false);
                opponentFocus = null;
            }
    }
    public float GetVelocityMagnitude()
    {
        // Debug.Log(characterMovement.Velocity.magnitude);
        return characterMovement.Velocity.magnitude;
    }

    public Vector3 GetForwardDirection()
    {
        return characterMovement.t_mesh.forward;
    }

    public Vector3 GetVelocityVector()
    {
        return characterMovement.Velocity;
    } 

    public FightMode GetFightMode(){return characterMovement.GetFightMode();}

    public MovementMode GetMovement(){return characterMovement.GetMovementMode();}

    public void SetFightmode(FightMode mode){characterMovement.SetFightMode(mode); }

    public void SetToRun(){characterMovement.SetMovementMode(MovementMode.Running);}

    public void SetToCrounch(){characterMovement.SetMovementMode(MovementMode.Crouching);}
    public void SetToWalk(){characterMovement.SetMovementMode(MovementMode.Walking);}

}
