using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class PlayerAnimationController : MonoBehaviour
{

    // [SerializeField] LayerMask _aimLayerMask;
    public Animator animator;
    private Character character;
    private CharacterMovement chrMovement;
    private bool playedHit = false;
    void Start()
    {
        character = GetComponent<Character>();
        chrMovement = GetComponent<CharacterMovement>();
    }
    // Start is called before the first frame update
    // void Awake() => animator = GetComponent<Animator>();
    
    // Update is called once per frame
    void LateUpdate()
    {
        if(animator==null) {Debug.LogWarning("No valid animator!"); return;}
        animator.SetFloat("Velocity", character.getVelocity());

        MovementMode currentMovement = character.GetMovement();
        // Debug.Log(currentMovement);
        if (currentMovement == MovementMode.Crouching)
            {
                animator.SetBool("isCrounch", true);
            }
        else
            {
                animator.SetBool("isCrounch", false);
            }
        FightMode current = character.GetFightMode();

        if(current == FightMode.Default)
        {
            animator.SetLayerWeight(1, chrMovement.GetHitLayerWeight());
            playedHit = false;
        }
        else if(current == FightMode.ReceiveDamage && !playedHit)
        {
            animator.SetLayerWeight(1, 1);
            animator.Play("Fight.Hit", 1, 0f);
            playedHit = true;
        }
    }
   
}
