
using UnityEngine;
using System;
public interface IOpponentReaction
{
    // Start is called before the first frame update
    void InvokeReaction(DamageType damageType, WeaponType holdMode, Transform player, float force, float time);

    // Action <Opponent> OnGotAttacked {get; set;} 
    Vector3 targetDirection {set; get;}

    HitBonus bonus {set; get;}

    Transform weapon {set; get;}

}
