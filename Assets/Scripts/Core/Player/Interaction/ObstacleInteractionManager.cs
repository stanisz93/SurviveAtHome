
using UnityEngine;
using DG.Tweening;

public class ObstacleInteractionManager : MonoBehaviour, IPlayerActionManager {
    
    public ObstacleInteraction obstacleInteraction;
  
    private PlayerTriggers playerTriggers;
    private CharacterMovement characterMovement;
    private PlayerAnimationController playerAnimationController;
    private Sequence MoveSequence;
    private Character character;

    private Endurance endurance;
    
    

    

    private void Start() {
        playerTriggers = GetComponent<PlayerTriggers>();
        characterMovement = GetComponent<CharacterMovement>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        character = GetComponent<Character>();
        endurance = GetComponent<Endurance>();

    }


    public void AssignObstacleInteraction(ObstacleInteraction obstacle, bool attach)
    {
            if(attach)
                obstacleInteraction = obstacle;
            else
                obstacleInteraction = null;
    }

    public Transform GetCurrentObstacle() => obstacleInteraction.transform;

    public bool IsEnduranceSatisfied()
    {
        int currentEndurance = endurance.GetCurrentEndurance();
        int expectedEndurance = obstacleInteraction.GetAnimType() switch 
        {
             ObstacleAnimType.slide => character.energyConsumption.slide,
             ObstacleAnimType.vault => character.energyConsumption.vaultOverBox,
             ObstacleAnimType.climbOverWall => character.energyConsumption.climbWall,
            ObstacleAnimType.PushDoor => character.energyConsumption.pushDoor,
            ObstacleAnimType.SlideUnderBed => 0,


        };
        if (currentEndurance >= expectedEndurance)
            {
                endurance.ReduceEndurance(expectedEndurance);
                return true;
            }
        else
            return false;
    }


    public void InteractObstacle()
    {
        obstacleInteraction.EstimateSide(characterMovement.t_mesh);
        Transform t_startPoint = obstacleInteraction.GetStartPoint();
        Transform t_endPoint = obstacleInteraction.GetEndPoint();

        int enduranceDecrease = 0;
        if(obstacleInteraction.AreConditionsSatisfied(character) && IsEnduranceSatisfied())
        {
            if(obstacleInteraction.forwardToObstacle)
                characterMovement.t_mesh.DORotate(Quaternion.LookRotation(t_startPoint.forward).eulerAngles, 0.2f);

            obstacleInteraction.PerformAction(playerAnimationController.animator, character);
  
            if(MoveSequence.IsActive())
                MoveSequence.Kill();
            MoveSequence = obstacleInteraction.RunSequence(transform, characterMovement.t_mesh);
            playerTriggers.ReleaseTriggerAfterSeconds(obstacleInteraction.ReleaseTime);
        }
        else
            playerTriggers.isTriggerEmpty = true;
    }

    public ObstacleAnimType GetCurrentAnimType()
    {
        return obstacleInteraction.GetAnimType();
    }

    public void InterruptMovementSequence(){
        if(MoveSequence != null)
            MoveSequence.Kill();
    }

}