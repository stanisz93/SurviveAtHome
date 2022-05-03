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

    public KickTrigger kickTrigger;

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
        StartCoroutine(ExposeToKickCollider());
        playerAnimationController.animator.SetTrigger("Kick");
        StartCoroutine(BlockMovement(triggerEmpty));
        StartCoroutine(ReleaseTrigger(1f));
        

        
    }

    public IEnumerator ExposeToKickCollider(float exposeTime=0.1f)
    {
        kickTrigger.SwitchCollider(true);
        yield return new WaitForSeconds(exposeTime);
        kickTrigger.SwitchCollider(false);
    }


        public IEnumerator BlockMovement(bool condition)
    {
        playerInput.blockMovement = true;
        yield return new WaitUntil(() => triggerEmpty);
        playerInput.blockMovement = false;

    }

    public void Die()
    {
            dying = true;
            playerAnimationController.animator.SetTrigger("Die");
            characterMovement.ResetVelocity();
            StartCoroutine(DieRoutine());
    }



    private IEnumerator DieRoutine()
    {       
            character.mainCamera.enabled = false;
            deathCamera.enabled = true;
            deathCamera.GetComponent<DeathCamera>().chooseCameraPosition();
            yield return new WaitForSeconds(2);

            
            playerAnimationController.animator.SetTrigger("resurrect");
            character.ResetPlayer();
            character.mainCamera.enabled = true;
            deathCamera.enabled = false;
            dying = false;
    }
}
