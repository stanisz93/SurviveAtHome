using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class PlayerAnimationController : MonoBehaviour
{

    // [SerializeField] LayerMask _aimLayerMask;
    public Animator animator;

    public float hitAnimLifeTime = 0.4f;
    public float hitFadeOutTime = 0.15f;

    private Character character;
    private CharacterMovement chrMovement;
    private LayerInvoker layerInvoker;
    private CapsuleCollider collider;
    private bool isHitAnimLaunch = false;

    void Start()
    {
        character = GetComponent<Character>();
        chrMovement = GetComponent<CharacterMovement>();
        layerInvoker = GetComponent<LayerInvoker>();
        collider = GetComponent<CapsuleCollider>();
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
        if(currentMovement == MovementMode.Sliding)
        {
           animator.SetTrigger("Slide");
        }
        
        if (currentMovement == MovementMode.Crouching)
            {
                animator.SetBool("isCrounch", true);
            }
        else
            {
                animator.SetBool("isCrounch", false);
            }
        FightMode current = character.GetFightMode();

        if(current == FightMode.ReceiveDamage && !isHitAnimLaunch)
        {
            StartCoroutine(ReceiveDamage());
            isHitAnimLaunch = true;
        }
    }



    IEnumerator ReceiveDamage()
    {
        layerInvoker.Reset();
        yield return layerInvoker.RunUntil("Fight.Hit", 1, hitAnimLifeTime);
        yield return layerInvoker.SmoothlyFadeOutLayer(hitFadeOutTime, 1);
        chrMovement.SetFightMode(FightMode.Default);
        isHitAnimLaunch = false;
    }


   
}
