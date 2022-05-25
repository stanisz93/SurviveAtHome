using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCollider : MonoBehaviour
{
    // Start is called before the first frame update
    public float BlockMovementWhileBump = 0.4f;
    public float MinSpeedToBump = 5.5f;
    private Collider collider;


    void Start()
    {
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision other) 
    {
        Opponent opponent = other.gameObject.GetComponent<Opponent>();
        if(opponent != null)
        {
            OpponentActions opponentActions = other.gameObject.GetComponent<OpponentActions>();
            if(opponentActions.GetOpponentMode() != OpponentMode.Faint && other.relativeVelocity.magnitude > MinSpeedToBump)
            {  
                ContactPoint contact = other.contacts[0];
                GetComponent<PlayerTriggers>().BumpOnZombie(contact.normal);
                opponent.GotTackled(contact.normal);
                StartCoroutine(gameObject.GetComponent<PlayerTriggers>().BlockMovementSeconds(BlockMovementWhileBump));
        
            }
        }
        gameObject.GetComponent<CharacterMovement>().Velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

    }
    void PlayerStop()
    {
        
    }
}
