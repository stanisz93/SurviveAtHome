using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class PlayerAnimationController : MonoBehaviour
{

    // [SerializeField] LayerMask _aimLayerMask;
    public Animator animator;

    public float hitAnimLifeTime = 0.4f;
    public float pickAnimLifeTime = 0.2f;
    public float pickFadeOutTime = 0.3f;
    public float hitFadeOutTime = 0.15f;


    private Character character;
    private CharacterMovement chrMovement;
    private LayerInvoker layerInvoker;
    private PlayerInput plrInput;
    private CapsuleCollider collider;
    private bool isHitAnimLaunch = false;
    private PlayerTriggers  plrTrigger;
  

    void Awake()
    {
        character = GetComponent<Character>();
        chrMovement = GetComponent<CharacterMovement>();
        layerInvoker = GetComponent<LayerInvoker>();
        collider = GetComponent<CapsuleCollider>();
        plrTrigger = GetComponent<PlayerTriggers>();
    }
    // Start is called before the first frame update
    // void Awake() => animator = GetComponent<Animator>();
    
    // Update is called once per frame
    void LateUpdate()
    {
        if(animator==null) {Debug.LogWarning("No valid animator!"); return;}
        animator.SetFloat("VelocityForward", character.GetVelocityForward());
        animator.SetFloat("VelocityRight", character.GetVelocityRight());
        animator.SetFloat("Velocity", character.GetVelocity());

        MovementMode currentMovement = character.GetMovement();

        // Debug.Log(currentMovement);
        character.triggeredAction?.Invoke();
        character.triggeredAction = null;
        
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
        layerInvoker.Reset(1);
        yield return layerInvoker.RunUntil("Fight.Hit", 1, hitAnimLifeTime);
        yield return layerInvoker.SmoothlyFadeOutLayer(hitFadeOutTime, 1);
        chrMovement.SetFightMode(FightMode.Default);
        isHitAnimLaunch = false;
    }

    public IEnumerator PickupItem()
    {
        layerInvoker.Reset(2);
        yield return layerInvoker.RunUntil("Pickup.PickingUp", 2, pickAnimLifeTime);
        yield return layerInvoker.SmoothlyFadeOutLayer(pickFadeOutTime, 2);
    }






   
}
