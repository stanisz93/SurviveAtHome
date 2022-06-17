using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggersManager : MonoBehaviour, IPlayerActionManager
{
    
    public float magnetMoveDelay = 0.4f;
    public AttackTrigger kickTrigger;
    private AttackTrigger currentAttackTrigger;

    private PlayerTriggers playerTriggers;
    private PlayerInput playerInput;
    private OpponentMagnet opponentMagnet;

    private Endurance endurance;

    private Character character;

    // Start is called before the first frame update
    void Start()
    {
        currentAttackTrigger = kickTrigger;
        playerInput = GetComponentInParent<PlayerInput>();
        playerTriggers = GetComponentInParent<PlayerTriggers>();
        opponentMagnet = GetComponentInChildren<OpponentMagnet>();
        endurance = GetComponentInParent<Endurance>();
        character = GetComponentInParent<Character>();
    }

    // Update is called once per frame

    public void ReleasePlayerControl()
    {
        playerTriggers.ReleaseTrigger();
        playerInput.blockMovement = false;
    }

    public void SetAttackable(IAttackable attackable)
    {
        currentAttackTrigger.SetAttackable(attackable); 
    }

    public IAttackable GetDefaultAttackable()
    {
        return currentAttackTrigger.GetDefaultAttackable();
    }

    public void StartAttack()
    {
        if(IsEnduranceSatisfied())
        {
            if (opponentMagnet.NearestOpponent != null)
            {
               opponentMagnet.MoveTowardNearestOpponent(currentAttackTrigger.GetDistanceLeft(), magnetMoveDelay);
            }
            currentAttackTrigger.ReleaseAttack();
        }
            
    }

    public bool IsEnduranceSatisfied()
    {
        int currentEndurance = endurance.GetCurrentEndurance();
        int expectedEndurance = currentAttackTrigger.GetAttackable().attackType switch
        {
            AttackType.Knife => character.energyConsumption.knifeAttack,
            AttackType.Stick => character.energyConsumption.stickAttack,
            AttackType.Kick => character.energyConsumption.kickAttack,
            AttackType.KickWhileVault => character.energyConsumption.kickWhileVault
        };
        if (currentEndurance >= expectedEndurance)
            {
                endurance.ReduceEndurance(expectedEndurance);
                return true;
            }
        else
            return false; 
    }
    

    public void ThrowWeapon()
    {
        if(currentAttackTrigger.isThrowable())
            currentAttackTrigger.Throw();
    }

    public void SetAttackTriggerCollider(AttackTrigger trigger)
    {
            currentAttackTrigger = trigger;
            // currentAttackTrigger.SetTriggerType(trigger.attackable.triggerType);
            // here either 
    }

    public void SetToDefaultAttackTrigger()
    {
        currentAttackTrigger = kickTrigger;
    }

    public AttackTrigger GetCurrentAttackTrigger()
    {
        return currentAttackTrigger;
    }

    public void TurnOffDamage()
    {
        currentAttackTrigger.hitTriggerCollider.enabled = false;
    }

        public void TurnOnDamage()
    {
        currentAttackTrigger.hitTriggerCollider.enabled = true;
        currentAttackTrigger.ResetHitOpponentsThisTurn();
    }

    
}
