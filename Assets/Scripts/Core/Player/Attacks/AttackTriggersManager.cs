using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggersManager : MonoBehaviour
{
    
    public float magnetMoveDelay = 0.4f;
    public AttackTrigger kickTrigger;
    private AttackTrigger currentAttackTrigger;

    private PlayerTriggers playerTriggers;
    private PlayerInput playerInput;
    private OpponentMagnet opponentMagnet;

    // Start is called before the first frame update
    void Start()
    {
        currentAttackTrigger = kickTrigger;
        playerInput = GetComponentInParent<PlayerInput>();
        playerTriggers = GetComponentInParent<PlayerTriggers>();
        opponentMagnet = GetComponentInChildren<OpponentMagnet>();
    }

    // Update is called once per frame

    public void ReleasePlayerControl()
    {
        playerTriggers.ReleaseTrigger();
        playerInput.blockMovement = false;
    }

    public void StartAttack()
    {
        if (opponentMagnet.NearestOpponent != null)
            {
                opponentMagnet.MoveTowardNearestOpponent(currentAttackTrigger.GetDistanceLeft(), magnetMoveDelay);
            }
            currentAttackTrigger.ReleaseAttack();
            
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
        currentAttackTrigger.GetComponent<Collider>().enabled = false;
    }

        public void TurnOnDamage()
    {
        currentAttackTrigger.GetComponent<Collider>().enabled = true;
        currentAttackTrigger.ResetHitOpponentsThisTurn();
    }

    
}
