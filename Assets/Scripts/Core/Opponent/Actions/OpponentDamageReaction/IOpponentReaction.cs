
using UnityEngine;
using System;
public interface IOpponentReaction
{
    // Start is called before the first frame update
    void InvokeReaction(WeaponType holdMode, Transform player, float force, float time);

    // Action <Opponent> OnGotAttacked {get; set;} 
    Vector3 targetDirection {set; get;}

    HitBonus bonus {set; get;}

    Transform weapon {set; get;}

}
