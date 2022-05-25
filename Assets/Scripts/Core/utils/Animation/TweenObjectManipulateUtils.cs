using UnityEngine;
class TweenObjectManipulateUtils
{
    
    // this method check before using tween to move and short it to the point of collision
    public static bool LimitMoveWhenObstacleWithinTrajectory(ref Vector3 expectedTranslation, Vector3 startPosition, Vector3 targetDir, float expectedMaxLength, LayerMask obstaclesMask)
    {
        RaycastHit hit;
         if (Physics.Raycast(startPosition, targetDir, out hit, expectedMaxLength, obstaclesMask))
            {
                Vector3 potentialPosition =  hit.point - startPosition;
                if(potentialPosition.magnitude < expectedTranslation.magnitude)
                    expectedTranslation = potentialPosition;
                return true;
            }
        return false;
    }
}