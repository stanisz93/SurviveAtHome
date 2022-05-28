using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEventController : MonoBehaviour
{

    public AttackTrigger defaultPushTrigger;
    public AttackTrigger currentAttackTrigger;

    // Start is called before the first frame update
    
    private Animator animator;
    private PlayerInput playerInput;
    private PlayerTriggers playerTriggers;
  
    void Start()
    {
        currentAttackTrigger = defaultPushTrigger;
        animator = GetComponent<Animator>();
        playerInput = GetComponentInParent<PlayerInput>();
        playerTriggers = GetComponentInParent<PlayerTriggers>();
    }

        
    public void TurnOnPushCollider()
    {
        currentAttackTrigger.GetComponent<Collider>().enabled = true;
        currentAttackTrigger.ResetHitOpponentsThisTurn();
    }

    public void TurnOffPushCollider()
    {
         currentAttackTrigger.GetComponent<Collider>().enabled = false;
    }
    public void ResetDefaultHoldPosition()
    {
        DefendItem stick = GetComponentInChildren<DefendItem>();
        if(stick != null)
        {
            stick.ChangeWeaponPositionToHold();
        }
    }


    public void SetAttackTriggerCollider(AttackTrigger trigger)
    {
            currentAttackTrigger = trigger;
    }

    public void SetToDefaultPushTrigger()
    {
        currentAttackTrigger = defaultPushTrigger;
    }

    public void ReleasePlayerControl()
    {
        playerTriggers.ReleaseTrigger();
        playerInput.blockMovement = false;
    }



    // Update is called once per frame
}
