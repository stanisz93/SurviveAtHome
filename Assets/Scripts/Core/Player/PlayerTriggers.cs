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
    private PlayerController playerInput;

    
    public bool dying = false;

    public bool isTriggerEmpty = true;

    ///DODGE
    public float dodgeMoveSide = 0.2f;
    public float dodgeMoveBack = 0.3f;
    public float dodgeDelayDotween = 0.1f;
    public float dodgeLookAt = 0.1f;


//Throwing params, to be moved 

    public Transform throwingHand;
    public float maxThrowDistance = 10f;

    public const float minDistance = 1f;
    public float currentThrowDistance = minDistance;
    public float throwDistanceSpeed = 5f;
    public float maxHeightOfThrow = 1f;
    
    // public Canvas ThrowProjectionCanvas;

    public bool isDebugObjOn = false;
    // private Vector3 targetThrowPos;
    AttackTriggersManager attackTriggerManager;
    private PlayerAnimatorEventController eventController;

    ///end of Throwing Params
    private HitBonus hitBonus;
    private OpponentMagnet opponentMagnet;

    // private bool isAimingToThrow = false;

    // Start is called before the first frame update
    void Start()
    {
        attackTriggerManager = GetComponentInChildren<AttackTriggersManager>();
        playerInput = GetComponent<PlayerController>();
        character = GetComponent<Character>();
        playerAnimationController = GetComponent<PlayerAnimationController>(); 
        characterMovement = GetComponent<CharacterMovement>();
        hitBonus = GetComponent<HitBonus>();

        // ThrowProjectionCanvas.enabled = false;
        eventController = GetComponentInChildren<PlayerAnimatorEventController>();
        opponentMagnet = GetComponentInChildren<OpponentMagnet>();
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



    // public void CancelThrow()
    // {
    //     ResetThrowState();
    //     playerInput.blockMovement = false;
    //     playerAnimationController.animator.SetTrigger("CancelAim");
    // }

    // public void ResetThrowState()
    // {
    //     ThrowProjectionCanvas.enabled = false;
    //     FinishAiming();
    // }

    // public void StartAim()
    // {
    //         // BlockMovement(true);
    //         playerAnimationController.animator.ResetTrigger("AimToThrow");
    //         playerAnimationController.animator.ResetTrigger("Throw");
    //         playerAnimationController.animator.SetTrigger("PrepareToAim");
    // }

    // public Vector3 GetThrowTargetPos()
    // {
    //     return targetThrowPos;
    // }

    // public void FinishAiming() => isAimingToThrow = false;

    // public IEnumerator ContinueAimToThrow()
    // { /// THis METHOD WAS USE TO MOVE POINTER OF THROW SO THAT PLAYER COULD THROW ANYWHERE
    ///I GUEES ITS NOT NEEDED, CHOOSING OPPONENT SHOULD BE ENOUGHT
    //     Debug.Log("Continue aiming");
    //     currentThrowDistance = minDistance;
    //     playerAnimationController.animator.SetTrigger("AimToThrow");
    //     ThrowProjectionCanvas.enabled = true;
    //     isAimingToThrow = true;
        
    //     while(isAimingToThrow)
    //     {
    //         if(currentThrowDistance < maxThrowDistance)
    //             currentThrowDistance += Time.deltaTime * throwDistanceSpeed;
    //         else
    //             currentThrowDistance = maxThrowDistance;
    //         RaycastHit hit;
    //         Vector3 aimPosition = throwingHand.position;
            
    //         aimPosition = new Vector3(aimPosition.x,maxHeightOfThrow, aimPosition.z);
    //         Vector3 characterPos = new Vector3(characterMovement.t_mesh.position.x, maxHeightOfThrow, characterMovement.t_mesh.position.z);
    //         Vector3 charTarget = characterPos + characterMovement.t_mesh.forward * currentThrowDistance;
    //         charTarget = new Vector3(charTarget.x, maxHeightOfThrow, charTarget.z);
    //         Vector3 handToTarget = charTarget - aimPosition;
    //         float fixedLength = handToTarget.magnitude; //distance from hand to target
    //         if (Physics.Raycast(aimPosition, handToTarget.normalized, out hit, fixedLength, ((1 << LayerMask.NameToLayer("Opponents") | (1 << LayerMask.NameToLayer("Obstacles"))))))
    //                 targetThrowPos = hit.point;
    //                 // ThrowProjectionCanvas.transform.rotation = Quaternion.LookRotation(hit.normal);
    //         else 
    //                 targetThrowPos = charTarget;

        
    //         if(isDebugObjOn)
    //             debugObj.position = targetThrowPos;
    //         ThrowProjectionCanvas.transform.position = targetThrowPos;
    //         var t_mesh = characterMovement.t_mesh.position;
    //         Vector3 lookPosition = new Vector3(t_mesh.x, targetThrowPos.y, t_mesh.z);
    //         ThrowProjectionCanvas.transform.LookAt(lookPosition); //Camera.main.transform.forward);
    //         yield return null;
    //     }
    // }

    public IEnumerator LeftBed()
    {
        BlockPlayerControl();
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

        character.TryToRunFightMode();
        float back = Vector3.Dot(-characterMovement.t_mesh.forward, character.GetVelocityDirection());
        float down = Vector3.Dot(characterMovement.t_mesh.right, character.GetVelocityDirection());
        float up = Vector3.Dot(-characterMovement.t_mesh.right, character.GetVelocityDirection());
        BlockPlayerControl();
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(dodgeDelayDotween);
        Vector3 opponentPos = characterMovement.t_mesh.position + 1.5f * characterMovement.t_mesh.forward;
        Vector3 opponentDir = characterMovement.t_mesh.forward;
        Vector3 dodgePos = Vector3.zero;
        if(opponentMagnet.NearestOpponent != null)
            {
                opponentPos = opponentMagnet.NearestOpponent.transform.position;
                opponentPos = new Vector3(opponentPos.x, characterMovement.t_mesh.position.y, opponentPos.z);
                opponentDir = (opponentPos - characterMovement.t_mesh.position).normalized;
            }
            
        if(up > back  && up > down)    
        {
            playerAnimationController.animator.SetTrigger("DodgeRight");
            playerAnimationController.animator.SetBool("DodgeDirection", true);
            Vector2 opponentXZ = new Vector2(opponentPos.x, opponentPos.z);
            Vector2 playerXZ = new Vector2(characterMovement.t_mesh.position.x, characterMovement.t_mesh.position.z);
            
            Vector2 circlePos = travelAlongCircle(playerXZ, opponentXZ, 1.2f);
            dodgePos = new Vector3(circlePos.x, characterMovement.t_mesh.position.y, circlePos.y);
            // TweenObjectManipulateUtils.LimitMoveWhenObstacleWithinTrajectory(ref dodgeDir, transform.position, dodgeDir,  1f, (1 << LayerMask.NameToLayer("Obstacles")));
            
            seq.Append(transform.DOMove(dodgePos, dodgeMoveSide));
        }
        else if(down > up && down > back)
        {
            playerAnimationController.animator.SetTrigger("DodgeRight");
            playerAnimationController.animator.SetBool("DodgeDirection", false);
            Vector2 opponentXZ = new Vector2(opponentPos.x, opponentPos.z);
            Vector2 playerXZ = new Vector2(characterMovement.t_mesh.position.x, characterMovement.t_mesh.position.z);
            
            Vector2 circlePos = travelAlongCircle(playerXZ, opponentXZ, -1.2f);
            dodgePos = new Vector3(circlePos.x, characterMovement.t_mesh.position.y, circlePos.y);
            seq.Append(transform.DOMove(dodgePos, dodgeMoveSide));
        
        }
        else
        {
            playerAnimationController.animator.SetTrigger("DodgeBack");
            // TweenObjectManipulateUtils.LimitMoveWhenObstacleWithinTrajectory(ref opponentDir, transform.position, - opponentDir.normalized,  1.5f, (1 << LayerMask.NameToLayer("Obstacles")));
            dodgePos = transform.position - 1.2f * opponentDir;
            seq.Append(transform.DOMove(dodgePos, dodgeMoveBack));
        }
        Vector3 finalDir = (dodgePos - characterMovement.t_mesh.position).normalized;
        // Vector3 rot = Quaternion.LookRotation(finalDir).eulerAngles;
        seq.Join(characterMovement.t_mesh.DODynamicLookAt(opponentPos, dodgeLookAt));
        
        


    }

public Vector2 travelAlongCircle(Vector2 pos, Vector2 center, float distance)
     {
         Vector3 axis = Vector3.back;
         Vector2 dir = pos - center;
         float circumference = 2.0f * Mathf.PI * dir.magnitude;
         float angle = distance / circumference * 360.0f;
         dir = Quaternion.AngleAxis(angle, axis) * dir;
         return dir + center;
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
