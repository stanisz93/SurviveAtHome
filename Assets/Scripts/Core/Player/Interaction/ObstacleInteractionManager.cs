
using UnityEngine;
using DG.Tweening;

public class ObstacleInteractionManager : MonoBehaviour {
    
      public ObstacleInteraction obstacleInteraction;

    private PlayerTriggers playerTriggers;
    private CharacterMovement characterMovement;
    private PlayerAnimationController playerAnimationController;
    private Sequence MoveSequence;
    private Character character;
    

    private void Start() {
        playerTriggers = GetComponent<PlayerTriggers>();
        characterMovement = GetComponent<CharacterMovement>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        character = GetComponent<Character>();
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
        if(obstacleInteraction.AreConditionsSatisfied(character))
        {
            // wcharacterMovement.t_mesh.rotation = Quaternion.LookRotation(t_startPoint.forward);
            if(obstacleInteraction.forwardToObstacle)
                characterMovement.t_mesh.DORotate(Quaternion.LookRotation(t_startPoint.forward).eulerAngles, 0.2f);
        
            if (obstacleInteraction.animType == ObstacleAnimType.slide)
                 playerAnimationController.animator.SetTrigger("Slide");
            else if(obstacleInteraction.animType == ObstacleAnimType.vault)
                {
                    playerAnimationController.animator.SetTrigger("Vault");
                    playerAnimationController.animator.SetBool("MirrorAnimation", Random.value > 0.5f); 
                }
            else if(obstacleInteraction.animType == ObstacleAnimType.climbOverWall)
                {
                    playerAnimationController.animator.SetTrigger("ClimbWall");
                }
            else if(obstacleInteraction.animType == ObstacleAnimType.PushDoor)
                {
                    playerAnimationController.animator.SetTrigger("PushDoor");
                    obstacleInteraction.ApplyForce(characterMovement.t_mesh.forward);

                }
            else
                Debug.Log("UnexpecetBehaviour!");
            if(MoveSequence.IsActive())
                MoveSequence.Kill();
            MoveSequence = obstacleInteraction.RunSequence(transform);
            playerTriggers.ReleaseTriggerAfterSeconds(obstacleInteraction.ReleaseTime);
        }
        else
            playerTriggers.isTriggerEmpty = true;
    }

    public void InterruptMovementSequence(){
        if(MoveSequence != null)
            MoveSequence.Kill();
    }

}