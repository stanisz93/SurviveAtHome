using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

public enum TriggerType{Melee, Distant};

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
        TriggerType.Distant => 3f,
        TriggerType.Melee => 1f,
    };
}

public void SetTriggerType(TriggerType triggerType)
{
    currentTriggerType = triggerType;
}


private void Start() {
    GetComponent<Collider>().enabled = false;
}

public void ResetHitOpponentsThisTurn()
{
    meetDuringTurn.Clear();
}

// private void OpponentDistantReaction(Opponent opponent)
// {
//             // defendItem.transform.parent = opponent.transform;
//             GetComponent<Collider>().enabled = false;
//             Vector3 force = (defendItem.transform.position - player.position).normalized * ForceOrRagdollPushWhileThrow;
//             opponent.GetComponent<KillUtils>().GotKilledByThrow(defendItem.transform, force);
// }

// private void OpponentMeleeReaction(Opponent opponent)
// {
//     DefendItem defendItem = GetComponentInParent<DefendItem>();
//     WeaponType weaponMode = WeaponType.Default;
//     if (defendItem != null){weaponMode = defendItem.holdMode;}
//         switch(weaponMode)
//         {
//             case(WeaponType.Knife):
//             {
//                 opponent.GotStabbed(player, pushForce, pushTime);
//                 break;
//             }
//             case(WeaponType.WoddenStick):
//             {
//                 opponent.GotPushed(player, pushForce, pushTime);
//                 opponent.SetKickPos(transform.position);
//                 break;
//             }
//             default:
//             {
//                 var superKick = false;
//                 if(bonus.GetBonusMode() == BonusMode.SuperKick)
//                     superKick = true;    
//                 opponent.GotPushed(player, pushForce, pushTime, superKick);
//                 opponent.SetKickPos(transform.position);
//                 break;
//             }
//         }
        
// }

WeaponType GetWeaponHoldType()
    {
        if (defendItem == null)
            return WeaponType.None;
        else
            return defendItem.weaponType;
    }


private void OnTriggerEnter(Collider other) {
        
         Opponent opponent = other.gameObject.GetComponent<Opponent>();
        if(opponent != null && !meetDuringTurn.Contains(opponent))
        {
            IOpponentReaction opponentReaction = null;
            if (currentTriggerType == TriggerType.Melee)
                opponentReaction = other.gameObject.GetComponent<MeleeReaction>() as IOpponentReaction;
            else if(currentTriggerType == TriggerType.Distant)
                opponentReaction = other.gameObject.GetComponent<DistanceAttackReaction>() as IOpponentReaction;
            
            if(opponentReaction != null)
            {
                opponentReaction.targetDirection = other.gameObject.transform.position - player.position;
                if(defendItem != null)
                    opponentReaction.weapon = defendItem.transform;
                opponentReaction.bonus = bonus;

                OpponentMode mode = other.gameObject.GetComponent<OpponentActions>().GetOpponentMode();
                if(mode != OpponentMode.Faint)
                {    
                    meetDuringTurn.Add(opponent);

                    OnHit?.Invoke(this);
                    opponentReaction.InvokeReaction(GetWeaponHoldType(), player, pushForce, pushTime);
                }
            }
        }
    }

}