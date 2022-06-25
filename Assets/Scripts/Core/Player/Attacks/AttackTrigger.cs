using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TriggerType{Melee, Distant};

public enum HitType{ManyEnemy, OneVictim};

public enum DamageType{Killing, NormalDamage, ToTheGround};
public class AttackTrigger : MonoBehaviour {

//delay of moving with dween
public float cameraShake = 1f;
public float pushForce = 1f;
public float pushTime = 0.1f;

public float ForceOrRagdollPushWhileThrow = 1f; //this is later multiplied by 1000

public Collider hitTriggerCollider;

[SerializeField]
private HitType hitType = HitType.ManyEnemy;
protected Transform player;

private HashSet<Opponent> meetDuringTurn; //add flag true if 
private HitBonus bonus;
private DefendItem defendItem;
private StressReceiver stressReceiver;


//this happend when hit is success
private Action<AttackTrigger> OnHit;
private TriggerType currentTriggerType;

private IAttackable attackable;

private IAttackable defaultAttackable;

private IThrowable throwable;

private Animator animator;
private PlayerTriggers playerTriggers;

private DamageType damageType = DamageType.NormalDamage;




private void Awake() {
    stressReceiver = GameObject.FindWithTag("MainCamera").GetComponent<StressReceiver>();
    defendItem = GetComponent<DefendItem>();
    player = GameObject.FindWithTag("PlayerMesh").transform;
    animator = player.GetComponent<Animator>();
    playerTriggers = player.GetComponentInParent<PlayerTriggers>();
    bonus = GameObject.FindWithTag("Player").GetComponent<HitBonus>();
    meetDuringTurn = new HashSet<Opponent>();
    OnHit += bonus.IncreaseCounts;
    OnHit += stressReceiver.InduceStressByHit;
    if(defendItem != null) //in case of neutral kick it wont happened
        OnHit += defendItem.ReduceEndurance;
    attackable = GetComponent<IAttackable>();
    defaultAttackable = GetComponent<IAttackable>();
    throwable = GetComponent<IThrowable>();
      
}


public float GetEnduranceMultiplier()
{
    return currentTriggerType switch
    {
        TriggerType.Distant => 5f,
        TriggerType.Melee => 1f,
    };
}

public bool isThrowable()
{
    return throwable != null;
}

public float GetDistanceLeft()
{
      if(currentTriggerType == TriggerType.Melee)
        {   
            return attackable.distanceLeft;
        }
    else
        return Mathf.Infinity;
}

public void SetDamageType(DamageType dType)
{
    damageType = dType;
}

public void SetTriggerType(TriggerType triggerType)
{
    currentTriggerType = triggerType;
}

public TriggerType GetCurrentTriggerType() => currentTriggerType;

public IAttackable GetDefaultAttackable()
{
    return defaultAttackable;
}



private void Start() {
    hitTriggerCollider.enabled = false;
}

public void ResetHitOpponentsThisTurn()
{
    meetDuringTurn.Clear();
}

public void Throw()
{
    throwable.Throw();
}


public void SetAttackable(IAttackable attackable)
{
    this.attackable = attackable;
}

public IAttackable GetAttackable()
{
    return attackable;
}

public void SetThrowTarget(Opponent opponent)
{
    if(throwable != null)
    {
        // Physics.IgnoreCollision(defendItem.physicsCollider, opponent.GetComponent<Collider>());
        throwable.SetThrowTarget(opponent.transform.position);
    }
    else
        Debug.Log("You are trying to throw, while this trigger doesn't have throwable");
}



public void ReleaseAttack()
{   
    attackable.ReleaseAttack();
    playerTriggers.BlockMovementSeconds(attackable.blockMovement);
    playerTriggers.ReleaseTriggerAfterSeconds(attackable.releaseTriggerTime);
    if(currentTriggerType == TriggerType.Melee)
        animator.SetTrigger(attackable.meleeAnimName);
    else if(currentTriggerType == TriggerType.Distant)
        if(throwable != null)
            animator.SetTrigger(throwable.distantAnimName);
    else
        Debug.Log("Unexpected. I assume that if weapon is throwable its also attackable!");
}


WeaponType GetWeaponHoldType()
    {
        if (defendItem == null)
            return WeaponType.None;
        else
            return defendItem.weaponType;
    }

public void InduceTrigger(Opponent opponent)
{

        if(opponent != null && !meetDuringTurn.Contains(opponent))
        {
            IOpponentReaction opponentReaction = null;
            if (currentTriggerType == TriggerType.Melee)
                opponentReaction = opponent.GetComponent<MeleeReaction>() as IOpponentReaction;
            else if(currentTriggerType == TriggerType.Distant)
                opponentReaction = opponent.GetComponent<DistanceAttackReaction>() as IOpponentReaction;
            
            if(opponentReaction != null)
            {
                if(hitType == HitType.OneVictim) 
                    {
                        if(defendItem != null)
                            defendItem.physicsCollider.enabled = false;

                        hitTriggerCollider.enabled = false;
                        
                    }

                opponentReaction.targetDirection = opponent.transform.position - player.position;
                if(defendItem != null)
                    opponentReaction.weapon = defendItem.transform;

                OpponentMode mode = opponent.GetComponent<OpponentActions>().GetOpponentMode();
                if(mode != OpponentMode.Faint)
                {    

                    if(bonus.GetBonusMode() == BonusMode.SuperKick)
                        damageType = DamageType.ToTheGround;
                    else
                        if(currentTriggerType == TriggerType.Melee)
                            damageType = DamageType.NormalDamage;
                    meetDuringTurn.Add(opponent);
                    opponentReaction.InvokeReaction(damageType, GetWeaponHoldType(), player, pushForce, pushTime);
                    OnHit?.Invoke(this);
                   }
            }
        }
}


private void OnTriggerEnter(Collider other) {
        
        if(hitTriggerCollider.enabled)
        {
            Opponent opponent = other.GetComponent<Opponent>();
            if (opponent != null)
            InduceTrigger(opponent);
        }


    }
}