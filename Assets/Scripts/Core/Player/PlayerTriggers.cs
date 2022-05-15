using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PlayerAnimationController), typeof(ItemPickupManager), typeof(Character))]
public class PlayerTriggers : MonoBehaviour
{

    public List<Transform> diePoints;
    public Camera deathCamera;

    private PlayerAnimationController playerAnimationController;
    private CharacterMovement characterMovement;
    private Character character;
    private PlayerInput playerInput;
    private Slidable slidable;
    public bool dying = false;

    public bool triggerEmpty = true;
    public Slidable Slidable { get => slidable; set => slidable = value; }


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        character = GetComponent<Character>();
        playerAnimationController = GetComponent<PlayerAnimationController>(); 
        characterMovement = GetComponent<CharacterMovement>();
    }

    public void Slide()
    {
        playerAnimationController.animator.SetTrigger("Slide");
        Vector3 t_slidePoint = slidable.GetSlidePoint();
        Vector3 t_landpoint = slidable.GetEndSlidePoint();
        Sequence sq = DOTween.Sequence();
        sq.Append(transform.DOMove(t_slidePoint, .1f));
        sq.Append(transform.DOMove(t_landpoint, .8f));
        StartCoroutine(ReleaseTrigger(1f));
    }

    IEnumerator ReleaseTrigger(float time)
    {
        yield return new WaitForSeconds(time);
        triggerEmpty = true;
    }
    public void Kick()
    {
        character.SpeedBeforeKick = character.GetVelocity();
        playerAnimationController.animator.SetTrigger("Kick");
        StartCoroutine(BlockMovement(triggerEmpty));
        StartCoroutine(ReleaseTrigger(0.8f));
    }

    public void StickAttack()
    {
        DefendItem defendItem = GetComponentInChildren<DefendItem>();
        defendItem.ChangeWeaponPositionToAttack();
        character.SpeedBeforeKick = character.GetVelocity();
        playerAnimationController.animator.SetTrigger("PushStick");
        StartCoroutine(BlockMovement(triggerEmpty));
        StartCoroutine(ReleaseTrigger(0.7f));
    }

    public void KnifeAttack()
    {
        DefendItem defendItem = GetComponentInChildren<DefendItem>();
        defendItem.ChangeWeaponPositionToAttack();
        character.SpeedBeforeKick = character.GetVelocity();
        playerAnimationController.animator.SetTrigger("Stab");
        StartCoroutine(ReleaseTrigger(0.4f));
    }



        public IEnumerator BlockMovement(bool condition)
    {
        playerInput.blockMovement = true;
        yield return new WaitUntil(() => triggerEmpty);
        playerInput.blockMovement = false;

    }

    public void Die()
    {
            if(!dying == true)
            {
                playerAnimationController.animator.SetTrigger("Die");
                characterMovement.ResetVelocity();
                StartCoroutine(DieRoutine());
            }
    }



    private IEnumerator DieRoutine()
    {       
            character.mainCamera.enabled = false;
            deathCamera.enabled = true;
            deathCamera.GetComponent<DeathCamera>().chooseCameraPosition();
            Health health = GetComponent<Health>();
            health.enabled = false;
            yield return new WaitForSeconds(2);

            
            playerAnimationController.animator.SetTrigger("resurrect");
            character.ResetPlayer();
            character.mainCamera.enabled = true;
            deathCamera.enabled = false;
            health.enabled = false;
            dying = false;
    }




}
