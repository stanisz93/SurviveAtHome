using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeReaction : MonoBehaviour, IOpponentReaction
{
    // Start is called before the first frame update

    public Vector3 targetDirection {set; get;}

    public Transform weapon {set; get;}
    private Opponent opponent;
    private void Start() {
        opponent = GetComponent<Opponent>();
    }
    
public void InvokeReaction(DamageType damageType, WeaponType holdMode, Transform sourceOfHit, float force, float pushTime)
{
    {
        switch(holdMode)
        {
            case(WeaponType.Knife):
            {
                opponent.GotStabbed(sourceOfHit, force, pushTime);
                break;
            }
            case(WeaponType.WoddenStick):
            {
                opponent.GotPushed(sourceOfHit, force, pushTime);
                opponent.SetDamagePosition(transform.position);
                break;
            }
            case(WeaponType.None):
            {   
                bool superHit = false;
                if(damageType == DamageType.ToTheGround)
                    superHit = true;
                opponent.GotPushed(sourceOfHit, force, pushTime, superHit);
                opponent.SetDefaultDamagePosition();
                break;
            }
        }
    }
}
        
}
