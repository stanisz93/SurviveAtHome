using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IObstacleInteractable
{
    //this metod should cointains whole process on player 
    // side to prepare to attack with different weapon
    void PerformAction(Animator animator, Character character); //usually just run sequence
    //but maybe something more
    

    ObstacleAnimType animType {get;}

    
    


}
