using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

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


//Throwing params, to be moved 

    public Transform throwingHand;
    public float maxThrowDistance = 10f;

    public const float minDistance = 1f;
    public float currentThrowDistance = minDistance;
    public float throwDistanceSpeed = 5f;
    public float maxHeightOfThrow = 1f;
    private bool isAimingStarted = false;
    
    public Canvas ThrowProjectionCanvas;

    public bool isDebugObjOn = false;
    public Transform debugObj;
    private Vector3 targetThrowPos;
    PlayerAnimatorEventController playerAnimatorEventController;


    ///end of Throwing Params
    private HitBonus hitBonus;


    // Start is called before the first frame update
    void Start()
    {
        playerAnimatorEventController = GetComponentInChildren<PlayerAnimatorEventController>();
        playerInput = GetComponent<PlayerInput>();
        character = GetComponent<Character>();
        playerAnimationController = GetComponent<PlayerAnimationController>(); 
        characterMovement = GetComponent<CharacterMovement>();
        hitBonus = GetComponent<HitBonus>();
        ThrowProjectionCanvas.enabled = false;
        debugObj.gameObject.SetActive(isDebugObjOn);
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

    public void BlockTrigger()
    {
        isTriggerEmpty = false;
    }
    public void BlockMovement(bool block=true)
    {
           playerInput.blockMovement = block;
    }


    public void RunReleaseTriggerRoutine(float time)
    {
        StartCoroutine(ReleaseTriggerAfterSeconds(time));
    }


    public void ReleaseThrow()
    {
        ResetThrowState();
        playerInput.blockMovement = false;
        playerAnimationController.animator.SetTrigger("CancelAim");
    }

    public void ResetThrowState()
    {
        ThrowProjectionCanvas.enabled = false;
        isAimingStarted = false;
    }

    public void StartAim()
    {
            BlockMovement(true);
            playerAnimationController.animator.ResetTrigger("AimToThrow");
            playerAnimationController.animator.ResetTrigger("Throw");
            playerAnimationController.animator.SetTrigger("PrepareToAim");
    }

    public Vector3 GetThrowTargetPos()
    {
        return targetThrowPos;
    }
    public void ContinueAimToThrow()
    {
        if(!isAimingStarted)
        {
            currentThrowDistance = minDistance;
            playerAnimationController.animator.SetTrigger("AimToThrow");
            ThrowProjectionCanvas.enabled = true;
            isAimingStarted = true;
        }
        if(currentThrowDistance < maxThrowDistance)
            currentThrowDistance += Time.deltaTime * throwDistanceSpeed;
        else
            currentThrowDistance = maxThrowDistance;
        RaycastHit hit;
        Vector3 aimPosition = throwingHand.position;
        
        aimPosition = new Vector3(aimPosition.x,maxHeightOfThrow, aimPosition.z);
        Vector3 characterPos = new Vector3(characterMovement.t_mesh.position.x, maxHeightOfThrow, characterMovement.t_mesh.position.z);
        Vector3 charTarget = characterPos + characterMovement.t_mesh.forward * currentThrowDistance;
        charTarget = new Vector3(charTarget.x, maxHeightOfThrow, charTarget.z);
        Vector3 handToTarget = charTarget - aimPosition;
        float fixedLength = handToTarget.magnitude; //distance from hand to target
        if (Physics.Raycast(aimPosition, handToTarget.normalized, out hit, fixedLength, ((1 << LayerMask.NameToLayer("Opponents") | (1 << LayerMask.NameToLayer("Obstacles"))))))
                targetThrowPos = hit.point;
                // ThrowProjectionCanvas.transform.rotation = Quaternion.LookRotation(hit.normal);
        else 
                targetThrowPos = charTarget;

    
        if(isDebugObjOn)
            debugObj.position = targetThrowPos;
        ThrowProjectionCanvas.transform.position = targetThrowPos;
        var t_mesh = characterMovement.t_mesh.position;
        Vector3 lookPosition = new Vector3(t_mesh.x, targetThrowPos.y, t_mesh.z);
        ThrowProjectionCanvas.transform.LookAt(lookPosition); //Camera.main.transform.forward);
        
    }

    public void ThrowWeapon() // later move it to be handled by each weapon script separatable
    {
        playerAnimatorEventController.currentAttackTrigger.SetTriggerType(TriggerType.Distant);
        playerAnimationController.animator.SetTrigger("Throw");
        StartCoroutine(BlockMovementUntilTriggerIsEmpty());
        StartCoroutine(ReleaseTriggerAfterSeconds(0.4f));
    }

    public void Kick()
    {
        
        character.SpeedBeforeKick = character.GetVelocityMagnitude();
        string kickType = hitBonus.GetBonusMode() == BonusMode.SuperKick ? "SuperKick" : "Kick";
        playerAnimationController.animator.SetTrigger(kickType);
        StartCoroutine(BlockMovementUntilTriggerIsEmpty());
        StartCoroutine(ReleaseTriggerAfterSeconds(0.8f));
    }


    public void StickAttack()
    {
        playerAnimatorEventController.currentAttackTrigger.SetTriggerType(TriggerType.Melee);
        DefendItem defendItem = GetComponentInChildren<DefendItem>();
        defendItem.ChangeWeaponPositionToAttack();
        character.SpeedBeforeKick = character.GetVelocityMagnitude();
        playerAnimationController.animator.SetTrigger("PushStick");
        StartCoroutine(BlockMovementSeconds(0.4f));
        StartCoroutine(ReleaseTriggerAfterSeconds(0.5f));
    }

    public void KnifeAttack()
    {
        playerAnimatorEventController.currentAttackTrigger.SetTriggerType(TriggerType.Melee);
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



        public IEnumerator BlockMovementUntilTriggerIsEmpty()
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
