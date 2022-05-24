
using UnityEngine;
using DG.Tweening;

public class ObstacleInteractionManager : MonoBehaviour {
    
      public ObstacleInteraction obstacleInteraction;

    private PlayerTriggers playerTriggers;
    private CharacterMovement characterMovement;
    private PlayerAnimationController playerAnimationController;
    private Sequence MoveSequence;

    private void Start() {
        playerTriggers = GetComponent<PlayerTriggers>();
        characterMovement = GetComponent<CharacterMovement>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
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
            MoveSequence.Kill();
            MoveSequence = DOTween.Sequence();
            MoveSequence.Append(transform.DOMove(t_startPoint.position, obstacleInteraction.MoveToPointTime));
            MoveSequence.Append(transform.DOMove(t_endPoint.position, obstacleInteraction.MoveToEndPointTime));
            playerTriggers.RunReleaseTriggerRoutine(obstacleInteraction.ReleaseTime);
        }
        else
            playerTriggers.isTriggerEmpty = true;
    }

    public void InterruptMovementSequence(){
        if(MoveSequence != null)
            MoveSequence.Kill();
    }

}