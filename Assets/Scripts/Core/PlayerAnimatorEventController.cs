using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEventController : MonoBehaviour
{

    public AttackTrigger kickTrigger;
    public AttackTrigger currentAttackTrigger;

    // Start is called before the first frame update
    public float TrailKickDecay = 0.1f;
    private Animator animator;
    private PlayerInput playerInput;
    private PlayerTriggers playerTriggers;

    private TrailRenderer kickTrail;

  
    void Start()
    {
        currentAttackTrigger = kickTrigger;
        animator = GetComponent<Animator>();
        playerInput = GetComponentInParent<PlayerInput>();
        playerTriggers = GetComponentInParent<PlayerTriggers>();
        kickTrail = GetComponentInChildren<TrailRenderer>();
        kickTrail.enabled = false;
    }

        
    public void TurnOnDamage()
    {
        currentAttackTrigger.GetComponent<Collider>().enabled = true;
        currentAttackTrigger.ResetHitOpponentsThisTurn();
    }

    
    IEnumerator TurnOffKickTrail()
    {
        yield return new WaitForSeconds(TrailKickDecay);
        kickTrail.enabled = false;
    }
    public void ToogleKickTrail()
    {
        kickTrail.enabled = true;
        StartCoroutine(TurnOffKickTrail());
    }

    public void TurnOffDamage()
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

    public void SetToDefaultAttackTrigger()
    {
        currentAttackTrigger = kickTrigger;
    }

    public void ReleasePlayerControl()
    {
        playerTriggers.ReleaseTrigger();
        playerInput.blockMovement = false;
    }



    // Update is called once per frame
}
