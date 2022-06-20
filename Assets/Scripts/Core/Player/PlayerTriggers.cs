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
    AttackTriggersManager attackTriggerManager;
    private PlayerAnimatorEventController eventController;

    ///end of Throwing Params
    private HitBonus hitBonus;
    private OpponentMagnet opponentMagnet;

    // Start is called before the first frame update
    void Start()
    {
        attackTriggerManager = GetComponentInChildren<AttackTriggersManager>();
        playerInput = GetComponent<PlayerInput>();
        character = GetComponent<Character>();
        playerAnimationController = GetComponent<PlayerAnimationController>(); 
        characterMovement = GetComponent<CharacterMovement>();
        hitBonus = GetComponent<HitBonus>();

        ThrowProjectionCanvas.enabled = false;
        eventController = GetComponentInChildren<PlayerAnimatorEventController>();
        opponentMagnet = GetComponentInChildren<OpponentMagnet>();
        debugObj.gameObject.SetActive(isDebugObjOn);
    }


    public void ReleaseTriggerAfterSeconds(float time)
    {
        StartCoroutine(ReleaseTriggerAfterSecondsCoroutine(time));
    }

    IEnumerator ReleaseTriggerAfterSecondsCoroutine(float time)
    {
        isTriggerEmpty = false;
        yield return new WaitForSeconds(time);
        isTriggerEmpty = true;
    }

    public bool IsTriggerEmpty() => isTriggerEmpty;

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



    public void CancelThrow()
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

    public IEnumerator LeftBed()
    {
        playerAnimationController.animator.SetTrigger("GetOutOfBed");
        Sequence moveSequence = DOTween.Sequence(); 
        Transform bed = character.GetCurrentObstacle();
        moveSequence.Append(characterMovement.t_mesh.DORotate(Quaternion.LookRotation(-bed.forward).eulerAngles, 0.4f));
        moveSequence.Join(character.transform.DOMove(bed.position + 1.5f * bed.forward, 1f));
        yield return moveSequence.WaitForCompletion();
        yield return new WaitUntil(() => eventController.IsAnimationFinished());
        ReleasePlayerControl();

    }

    public void ReleasePlayerControl()
    {
        ReleaseTrigger();
        playerInput.blockMovement = false;
    }

    void BlockPlayerControl()
    {
        BlockTrigger();
        playerInput.blockMovement = true;
    }

    public void Dodge()
    {
        playerAnimationController.animator.SetTrigger("DodgeBack");
        Vector3 dodgeDir = 1.5f * characterMovement.t_mesh.forward;
        if(opponentMagnet.NearestOpponent != null)
            {
                dodgeDir = opponentMagnet.NearestOpponent.transform.position - characterMovement.t_mesh.position;
                dodgeDir = 1.5f * new Vector3(dodgeDir.x, 0f, dodgeDir.z).normalized;
            }
            
        BlockPlayerControl();
        Sequence seq = DOTween.Sequence();
        TweenObjectManipulateUtils.LimitMoveWhenObstacleWithinTrajectory(ref dodgeDir, transform.position, - dodgeDir.normalized,  1.5f, (1 << LayerMask.NameToLayer("Obstacles")));
        seq.Append(transform.DOMove(transform.position - dodgeDir, 0.3f));
        Vector3 rot = Quaternion.LookRotation(dodgeDir.normalized).eulerAngles;
        seq.Join(characterMovement.t_mesh.DORotate(rot, .1f));
        


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
        BlockMovementSeconds(0.4f);

        
    }

    

    IEnumerator BlockMovementCoroutine(float duration)
    {
        playerInput.blockMovement = true;
        yield return new WaitForSeconds(duration);
        playerInput.blockMovement = false;
    }
    public void BlockMovementSeconds(float duration)
    {
        StartCoroutine(BlockMovementCoroutine(duration));

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
