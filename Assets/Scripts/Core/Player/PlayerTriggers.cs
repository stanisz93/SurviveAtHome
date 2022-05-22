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

    public bool isTriggerEmpty = true;
    public Slidable Slidable { get => slidable; set => slidable = value; }
    public ObstacleInteraction obstacleInteraction;

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


    public void AssignObstacleInteraction(ObstacleInteraction obstacle, bool attach)
    {
            if(attach)
                obstacleInteraction = obstacle;
            else
                obstacleInteraction = null;
    }

    public void InteractObstacle()
    {
        obstacleInteraction.EstimateSide(characterMovement.t_mesh);
        Transform t_startPoint = obstacleInteraction.GetStartPoint();
        Transform t_endPoint = obstacleInteraction.GetEndPoint();
        float dot = Vector3.Dot(characterMovement.Velocity.normalized, t_startPoint.forward);
        if(dot > obstacleInteraction.interactThreshold)
        {
            characterMovement.t_mesh.rotation = Quaternion.LookRotation(t_startPoint.forward);
            if (obstacleInteraction.animType == ObstacleAnimType.slide)
                 playerAnimationController.animator.SetTrigger("Slide");
            else if(obstacleInteraction.animType == ObstacleAnimType.vault)
                playerAnimationController.animator.SetTrigger("Vault");
            else
                Debug.Log("UnexpecetBehaviour!");
            Sequence sq = DOTween.Sequence();
            sq.Append(transform.DOMove(t_startPoint.position, obstacleInteraction.MoveToPointTime));
            sq.Append(transform.DOMove(t_endPoint.position, obstacleInteraction.MoveToEndPointTime));
            StartCoroutine(ReleaseTrigger(obstacleInteraction.ReleaseTime));
        }
        else
            isTriggerEmpty = true;
    }

    IEnumerator ReleaseTrigger(float time)
    {
        yield return new WaitForSeconds(time);
        isTriggerEmpty = true;
    }
    public void Kick()
    {
        
        character.SpeedBeforeKick = character.GetVelocity();
        string kickType = hitBonus.GetBonusMode() == BonusMode.SuperKick ? "SuperKick" : "Kick";
        playerAnimationController.animator.SetTrigger(kickType);
        StartCoroutine(BlockMovement(isTriggerEmpty));
        StartCoroutine(ReleaseTrigger(0.8f));
    }


    public void StickAttack()
    {
        DefendItem defendItem = GetComponentInChildren<DefendItem>();
        defendItem.ChangeWeaponPositionToAttack();
        character.SpeedBeforeKick = character.GetVelocity();
        playerAnimationController.animator.SetTrigger("PushStick");
        StartCoroutine(BlockMovementAfter(0.4f));
        StartCoroutine(ReleaseTrigger(0.5f));
    }

    public void KnifeAttack()
    {
        DefendItem defendItem = GetComponentInChildren<DefendItem>();
        defendItem.ChangeWeaponPositionToAttack();
        character.SpeedBeforeKick = character.GetVelocity();
        playerAnimationController.animator.SetTrigger("Stab");
        StartCoroutine(ReleaseTrigger(0.4f));
    }



        public IEnumerator BlockMovement(bool condition)
    {
        playerInput.blockMovement = true;
        yield return new WaitUntil(() => isTriggerEmpty);
        playerInput.blockMovement = false;

    }
    public IEnumerator BlockMovementAfter(float duration)
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
