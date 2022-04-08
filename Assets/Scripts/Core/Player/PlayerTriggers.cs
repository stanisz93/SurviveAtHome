using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PlayerAnimationController), typeof(Character))]
public class PlayerTriggers : MonoBehaviour
{

    private PlayerAnimationController playerAnimationController;
    private CharacterMovement characterMovement;
    private Character character;
    private Slidable slidable;
    public bool dying = false;
    public Slidable Slidable { get => slidable; set => slidable = value; }


    // Start is called before the first frame update
    void Start()
    {
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

    private IEnumerator DieRoutine()
    {       
            yield return new WaitForSeconds(2);
            playerAnimationController.animator.SetTrigger("resurrect");
            character.ResetPlayer();
            dying = false;
    }
}
