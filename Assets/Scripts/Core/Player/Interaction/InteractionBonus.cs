using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBonus : MonoBehaviour {

        // public float delayWhenVaultKick = 0.1f;

    // private Animator animator;
    // private PlayerInput playerInput; 
    public VaultKickAttack vaultKickAttackable;
    private OpponentMagnet opponentMagnet;
    private AttackTriggersManager attackTriggersManager;
    private HitBonus hitBonus;
    public bool isVaultContext = false;

    private IAttackable defaultAttackable;
    // private bool allowToKick = false;
    private PlayerTriggers playerTriggers;

    private void Start() {
        // playerInput = GetComponentInParent<PlayerInput>();
        attackTriggersManager = GetComponent<AttackTriggersManager>();
        opponentMagnet = GetComponentInChildren<OpponentMagnet>();
        // animator = GetComponent<Animator>();
        hitBonus = GetComponentInParent<HitBonus>();
        playerTriggers = GetComponentInParent<PlayerTriggers>();
  }

    // public void TurnOnBonusInteraction()
    // {
    //     allowToKick = true;
    // }

    // public IEnumerator SetToKickWhileVault()
    // {
    //     // if (allowToKick)
    //     // {
    //         hitBonus.SetSuperKick();
    //         // yield return new WaitForSeconds(delayWhenVaultKick);
            
    //         // animator.SetTrigger("KickWhileVault");
            
            
    //         // if(opponentMagnet.NearestOpponent != null)
    //         // {
    //         //     // allowToKick = false;
    //         //     Time.timeScale = 1f;
    //             opponentMagnet.MoveTowardNearestOpponent();
    //         }
    //     // }
    // }

    public void TurnOnVaultBonus()
    {
        // Debug.Log("TurnOnVault");
        defaultAttackable = attackTriggersManager.GetCurrentAttackable();
        attackTriggersManager.SetAttackable((IAttackable)vaultKickAttackable);
        isVaultContext = true;
        vaultKickAttackable.enabled = true;
    }

    public IEnumerator UntilVault()
    {
        yield return new WaitUntil(() => isVaultContext);
    }

    public void TurnOffVaultBonus()
    {
        // Debug.Log("TurnOffVault");
        vaultKickAttackable.enabled = false;
        attackTriggersManager.TurnOffDamage();
        attackTriggersManager.SetAttackable(defaultAttackable);
        hitBonus.RemoveSuperKick();
        isVaultContext = false;
    }
    
}