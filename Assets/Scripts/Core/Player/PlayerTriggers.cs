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
    private Slidable slidable;

    private ItemPickupManager itemPickupManager;
    public bool dying = false;
    public Slidable Slidable { get => slidable; set => slidable = value; }


    // Start is called before the first frame update
    void Start()
    {
        itemPickupManager = GetComponent<ItemPickupManager>();
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
        slidable = null;
    }

    public void Die()
    {
            dying = true;
            playerAnimationController.animator.SetTrigger("Die");
            characterMovement.ResetVelocity();
            StartCoroutine(DieRoutine());
    }

    public void PickItem()
    {
        SpoonItem best = itemPickupManager.GetBestOption();
        if(best != null)
        {
            itemPickupManager.RemovePotentialObject(best);
            best.RunPickEvent();
        }

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
