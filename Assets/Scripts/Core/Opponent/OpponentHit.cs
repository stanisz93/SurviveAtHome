using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentHit : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) 
    {
    
        Health health = other.GetComponent<Health>();
        if(health != null)
        {
            Debug.Log("Hit playe!");
            health.TakeDamage(gameObject.GetComponentInParent<Opponent>().damage);

        }
    }

}
