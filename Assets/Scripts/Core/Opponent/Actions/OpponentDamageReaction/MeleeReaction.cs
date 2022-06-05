using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeReaction : MonoBehaviour, IOpponentReaction
{
    // Start is called before the first frame update

    public Vector3 targetDirection {set; get;}

    public HitBonus bonus {set; get;}
    public Transform weapon {set; get;}
    private Opponent opponent;
    private void Start() {
        opponent = GetComponent<Opponent>();
    }
    
public void InvokeReaction(DamageType damageType, WeaponType holdMode, Transform player, float force, float pushTime)
{
    {
        switch(holdMode)
        {
            case(WeaponType.Knife):
            {
                opponent.GotStabbed(player, force, pushTime);
                break;
            }
            case(WeaponType.WoddenStick):
            {
                opponent.GotPushed(player, force, pushTime);
                opponent.SetKickPos(transform.position);
                break;
            }
            case(WeaponType.None):
            {
                var superKick = false;
                if(bonus.GetBonusMode() == BonusMode.SuperKick)
                    superKick = true;    
                opponent.GotPushed(player, force, pushTime, superKick);
                opponent.SetKickPos(transform.position);
                break;
            }
        }
    }
}
        
}
