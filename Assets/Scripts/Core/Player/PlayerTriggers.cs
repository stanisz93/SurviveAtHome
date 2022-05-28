using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PlayerAnimationController), typeof(ItemPickupManager), typeof(Character))]
public class PlayerTriggers : MonoBehaviour
{

    public List<Transform> diePoints;
    public Camera deathCamera;
    public LayerMask ObstaclesMask;

    private PlayerAnimationController playerAnimationController;
    private CharacterMovement characterMovement;
    private Character character;
    private PlayerInput playerInput;

    
    public bool dying = false;

    public bool isTriggerEmpty = true;

    private HitBonus hitBonus;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        character = GetComponent<Character>();
        playerAnimationController = GetComponent<PlayerAnimationController>(); 
        characterMovement = GetComponent<CharacterMovement>();
        hitBonus = GetComponent<HitBonus>();
    }


    public IEnumerator ReleaseTriggerAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        isTriggerEmpty = true;
    }

    public void ReleaseTrigger()
    {
        isTriggerEmpty = true;
    }




    public void RunReleaseTriggerRoutine(float time)
    {
        StartCoroutine(ReleaseTriggerAfterSeconds(time));
    }
    public void Kick()
    {
        
        character.SpeedBeforeKick = character.GetVelocityMagnitude();
        string kickType = hitBonus.GetBonusMode() == BonusMode.SuperKick ? "SuperKick" : "Kick";
        playerAnimationController.animator.SetTrigger(kickType);
        StartCoroutine(BlockMovement());
        StartCoroutine(ReleaseTriggerAfterSeconds(0.8f));
    }


    public void StickAttack()
    {
        DefendItem defendItem = GetComponentInChildren<DefendItem>();
        defendItem.ChangeWeaponPositionToAttack();
        character.SpeedBeforeKick = character.GetVelocityMagnitude();
        playerAnimationController.animator.SetTrigger("PushStick");
        StartCoroutine(BlockMovementSeconds(0.4f));
        StartCoroutine(ReleaseTriggerAfterSeconds(0.5f));
    }

    public void KnifeAttack()
    {
        DefendItem defendItem = GetComponentInChildren<DefendItem>();
        defendItem.ChangeWeaponPositionToAttack();
        character.SpeedBeforeKick = character.GetVelocityMagnitude();
        playerAnimationController.animator.SetTrigger("Stab");
        StartCoroutine(ReleaseTriggerAfterSeconds(0.4f));
    }

    public void BumpOnZombie(Vector3 collisionNormal)
    {
        Transform player = GameObject.FindWithTag("PlayerMesh").transform;
        Vector3 direction = Vector3.Cross(player.up, collisionNormal);
        playerAnimationController.animator.SetTrigger("BumpOnZombie");
        player.rotation = Quaternion.LookRotation(direction);
        Vector3 plannedDir = 1.3f * direction;
        bool hitObstacle = TweenObjectManipulateUtils.LimitMoveWhenObstacleWithinTrajectory(ref plannedDir, player.position, direction, 1.1f * plannedDir.magnitude, ObstaclesMask);
        transform.DOMove(transform.position + plannedDir, 0.4f);
        StartCoroutine(BlockMovementSeconds(0.4f));

        
    }



        public IEnumerator BlockMovement()
    {
        playerInput.blockMovement = true;
        yield return new WaitUntil(() => isTriggerEmpty);
        playerInput.blockMovement = false;

    }
    public IEnumerator BlockMovementSeconds(float duration)
    {
        playerInput.blockMovement = true;
        yield return new WaitForSeconds(duration);
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
