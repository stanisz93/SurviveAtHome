using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEventController : MonoBehaviour
{

    public AttackTrigger defaultPushTrigger;
    public AttackTrigger currentAttackTrigger;
    public float delayWhenVaultKick = 0.1f;
    private bool kickedwhileVault = false;
    // Start is called before the first frame update
    private PlayerInput playerInput; 
    private Animator animator;

    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
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

    public IEnumerator SetToKickWhileVault()
    {
        yield return new WaitForSeconds(delayWhenVaultKick);
        kickedwhileVault = true;
        animator.SetTrigger("KickWhileVault");
    }

    public void TurnOnVaultBonus()
    {
        playerInput.isVaultContext = true;
    }

    public void TurnOffVaultBonus()
    {
        TurnOffPushCollider();
        playerInput.isVaultContext = false;
    }


    public void SetAttackTriggerCollider(AttackTrigger trigger)
    {
            currentAttackTrigger = trigger;
    }

    public void SetToDefaultPushTrigger()
    {
        currentAttackTrigger = defaultPushTrigger;
    }

    public void KickOpponentWhileVault()
    {
        if(kickedwhileVault)
            TurnOnPushCollider();
        kickedwhileVault = false;
    }


    // Update is called once per frame
}
