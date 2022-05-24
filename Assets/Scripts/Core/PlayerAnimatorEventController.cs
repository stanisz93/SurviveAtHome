using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEventController : MonoBehaviour
{

    public AttackTrigger defaultPushTrigger;
    public AttackTrigger currentAttackTrigger;

    // Start is called before the first frame update
    
    private Animator animator;
  
    void Start()
    {
        currentAttackTrigger = defaultPushTrigger;
        animator = GetComponent<Animator>();
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



    // Update is called once per frame
}
