using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AttackType {Knife, Stick, Kick, KickWhileVault};
public interface IAttackable
{
    //this metod should cointains whole process on player 
    // side to prepare to attack with different weapon
    TriggerType triggerType {get;}


    //time that need to pass to have control over player again
    float blockMovement {get;}

    AttackType attackType {get;}

    string meleeAnimName {get;}
    //time when another trigger/action could be run
    float releaseTriggerTime {get;}

    //distance between you and opponent while performing attack
    float distanceLeft {get;}
    void ReleaseAttack();
    


    


}
