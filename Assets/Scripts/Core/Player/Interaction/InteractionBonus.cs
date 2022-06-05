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
    private bool allowToKick = false;

    private void Start() {
        playerInput = GetComponentInParent<PlayerInput>();
        playerAnimatorEventController = GetComponent<PlayerAnimatorEventController>();
        opponentMagnet = GetComponentInParent<OpponentMagnet>();
        animator = GetComponent<Animator>();
    }

    public void TurnOnBonusInteraction()
    {
        allowToKick = true;
    }

    public IEnumerator SetToKickWhileVault()
    {
        if (allowToKick)
        {
            yield return new WaitForSeconds(delayWhenVaultKick);
            animator.SetTrigger("KickWhileVault");
            
            if(opponentMagnet.NearestOpponent != null)
            {
                allowToKick = false;
                Time.timeScale = 1f;
                opponentMagnet.MoveTowardNearestOpponent();
            }
        }
    }

    public void TurnOnVaultBonus()
    {
        isVaultContext = true;
        if(opponentMagnet.NearestOpponent != null && allowToKick)
        {
            Time.timeScale = 0.1f;
        }
    }

    public void TurnOffVaultBonus()
    {
        playerAnimatorEventController.TurnOffDamage();
        isVaultContext = false;
        Time.timeScale = 1f;
    }
    
}