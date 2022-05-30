using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OpponentHit : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Transform hittingArm;
    private bool isDamageTaken = false;
    private Collider hitCollider;
    private bool isInitialEnable = true; // this flag first enable when create object
    private void Start() {
        hitCollider = GetComponent<Collider>();
        hitCollider.enabled = false;
    }
    private void OnEnable() {
        if(isInitialEnable)
            isInitialEnable = false;

        else
        {
            transform.SetParent(hittingArm, true);
            transform.localPosition = Vector3.zero;
            hitCollider.enabled = true;
        }
            
        isDamageTaken = false;
    }

    private void OnDisable() {
        hitCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other) 
    {
    
        Health health = other.GetComponent<Health>();
        if(health != null && !isDamageTaken)
        {
            Opponent opponent = gameObject.GetComponentInParent<Opponent>();
            if(opponent != null)
            {
                health.TakeDamage(gameObject.GetComponentInParent<Opponent>().damage);
                isDamageTaken = true;
                transform.parent = null;
            }
        }
    }

}
