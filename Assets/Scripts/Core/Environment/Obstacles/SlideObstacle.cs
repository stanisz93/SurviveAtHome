using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideObstacle : MonoBehaviour, IObstacleInteractable
{
    // Start is called before the first frame update

    public ObstacleAnimType animType{ get {return ObstacleAnimType.slide;}}

    public void PerformAction(Animator animator, Character character)
    {
        animator.SetTrigger("Slide");
    }
}
