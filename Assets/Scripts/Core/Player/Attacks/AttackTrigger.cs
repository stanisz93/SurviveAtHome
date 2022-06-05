using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TriggerType{Melee, Distant};

public enum HitType{ManyEnemy, OneVictim};

public enum DamageType{Killing, NormalDamage};
public class AttackTrigger : MonoBehaviour {

//delay of moving with dween
public float cameraShake = 1f;
public float pushForce = 1f;
public float pushTime = 0.1f;

public float ForceOrRagdollPushWhileThrow = 1f; //this is later multiplied by 1000

private Collider hitTriggerCollider;
protected Transform player;

private HashSet<Opponent> meetDuringTurn; //add flag true if 
private HitBonus bonus;
private DefendItem defendItem;
private StressReceiver stressReceiver;

private Action<AttackTrigger> OnHit;
private TriggerType currentTriggerType;
private HitType hitType = HitType.ManyEnemy;

private DamageType damageType = DamageType.NormalDamage;




private void Awake() {
    stressReceiver = GameObject.FindWithTag("MainCamera").GetComponent<StressReceiver>();
    defendItem = GetComponentInParent<DefendItem>();
    player = GameObject.FindWithTag("PlayerMesh").transform;
    bonus = GameObject.FindWithTag("Player").GetComponent<HitBonus>();
    hitTriggerCollider = GetComponent<Collider>();
    meetDuringTurn = new HashSet<Opponent>();
    OnHit += bonus.IncreaseCounts;
    OnHit += stressReceiver.InduceStressByHit;
    if(defendItem != null) //in case of neutral kick it wont happened
        OnHit += defendItem.ReduceEndurance;
      
}


public float GetEnduranceMultiplier()
{
    return currentTriggerType switch
    {
        TriggerType.Distant => 5f,
        TriggerType.Melee => 1f,
    };
}

public void SetDamageType(DamageType dType)
{
    damageType = dType;
}

public void SetTriggerType(TriggerType triggerType)
{
    currentTriggerType = triggerType;
}

public void SetHitType(HitType hitType)
{
    this.hitType = hitType;
}


private void Start() {
    GetComponent<Collider>().enabled = false;
}

public void ResetHitOpponentsThisTurn()
{
    hitType = HitType.ManyEnemy;
    meetDuringTurn.Clear();
}


WeaponType GetWeaponHoldType()
    {
        if (defendItem == null)
            return WeaponType.None;
        else
            return defendItem.weaponType;
    }

public void InduceTrigger(GameObject gameObject)
{

    Opponent opponent = gameObject.GetComponent<Opponent>();
        if(opponent != null && !meetDuringTurn.Contains(opponent))
        {
            IOpponentReaction opponentReaction = null;
            if (currentTriggerType == TriggerType.Melee)
                opponentReaction = gameObject.GetComponent<MeleeReaction>() as IOpponentReaction;
            else if(currentTriggerType == TriggerType.Distant)
                opponentReaction = gameObject.GetComponent<DistanceAttackReaction>() as IOpponentReaction;
            
            if(opponentReaction != null)
            {
                if(hitType == HitType.OneVictim) 
                    {
                        hitTriggerCollider.enabled = false;
                        defendItem.interactCollider.enabled = true;
                    }

                opponentReaction.targetDirection = gameObject.transform.position - player.position;
                if(defendItem != null)
                    opponentReaction.weapon = defendItem.transform;
                opponentReaction.bonus = bonus;

                OpponentMode mode = gameObject.GetComponent<OpponentActions>().GetOpponentMode();
                if(mode != OpponentMode.Faint)
                {    
                    meetDuringTurn.Add(opponent);
                    opponentReaction.InvokeReaction(damageType, GetWeaponHoldType(), player, pushForce, pushTime);
                    OnHit?.Invoke(this);
                   }
            }
        }
}


private void OnTriggerEnter(Collider other) {
        

       InduceTrigger(other.gameObject);

    }

}