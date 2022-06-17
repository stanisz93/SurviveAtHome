using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCollider : MonoBehaviour
{
    // Start is called before the first frame update
    public float BlockMovementWhileBump = 0.4f;
    public float MinSpeedToBump = 3.5f;

    public float MinSpeedToPlayerAlmostFall = 5.5f;
    private Collider collider;

    private PlayerTriggers plrTriggers;


    void Start()
    {
        collider = GetComponent<Collider>();
        plrTriggers = GetComponent<PlayerTriggers>();
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision other) 
    {
        Opponent opponent = other.gameObject.GetComponent<Opponent>();
        if(opponent != null)
        {
            OpponentActions opponentActions = other.gameObject.GetComponent<OpponentActions>();
            if(opponentActions.GetOpponentMode() != OpponentMode.Faint)
            {  
                ContactPoint contact = other.contacts[0];
                if(other.relativeVelocity.magnitude > MinSpeedToBump)
                    {
                        plrTriggers.BlockMovementSeconds(BlockMovementWhileBump);
                        if(opponentActions.GetOpponentMode() != OpponentMode.Attacking)
                            opponent.GotTackled(contact.normal);
                        if(other.relativeVelocity.magnitude > MinSpeedToPlayerAlmostFall)
                            plrTriggers.BumpOnZombie(contact.normal);
                    }
            }
            else
            {
                gameObject.GetComponent<PlayerTriggers>().BlockMovementSeconds(BlockMovementWhileBump / 10.0f);
            }
        }
         gameObject.GetComponent<PlayerTriggers>().BlockMovementSeconds(BlockMovementWhileBump / 10.0f);

    }
    void PlayerStop()
    {
        
    }
}
