using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClimbOverObstacle : MonoBehaviour, IObstacleInteractable
{
    // Start is called before the first frame update

    public ObstacleAnimType animType{ get {return ObstacleAnimType.climbOverWall;}}

    public void PerformAction(Animator animator, Character character)
    {
        animator.SetTrigger("ClimbWall");        
    }
}