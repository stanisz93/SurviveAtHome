using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBonus : MonoBehaviour {

        public float delayWhenVaultKick = 0.1f;
    private OpponentMagnet opponentMagnet;
    private Animator animator;
    private PlayerInput playerInput; 
    private PlayerAnimatorEventController playerAnimatorEventController;
    public bool isVaultContext = false;
    private bool allowToKick = true;

    private void Start() {
        playerInput = GetComponentInParent<PlayerInput>();
        playerAnimatorEventController = GetComponent<PlayerAnimatorEventController>();
        opponentMagnet = GetComponentInChildren<OpponentMagnet>();
        animator = GetComponent<Animator>();
    }

    public IEnumerator SetToKickWhileVault()
    {
        if (allowToKick)
        {
            allowToKick = false;
            yield return new WaitForSeconds(delayWhenVaultKick);
            animator.SetTrigger("KickWhileVault");
            
            if(opponentMagnet.NearestOpponent != null)
            {
                Time.timeScale = 1f;
                opponentMagnet.MoveTowardNearestOpponent();
            }
        }
    }

    public void TurnOnVaultBonus()
    {
        isVaultContext = true;
        if(opponentMagnet.NearestOpponent != null)
        {
            Time.timeScale = 0.1f;
        }
    }

    public void TurnOffVaultBonus()
    {
        playerAnimatorEventController.TurnOffPushCollider();
        isVaultContext = false;
        Time.timeScale = 1f;
        allowToKick = true;
    }
    
}