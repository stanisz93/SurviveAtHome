using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject Zombie;

    public Rigidbody ragdollHead;

    public bool isRagdoll = false;
    private Rigidbody[] rigidBodies;
    private Collider[] ragdollColliders;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rigidBodies = Zombie.GetComponentsInChildren<Rigidbody>();
        ragdollColliders = Zombie.GetComponentsInChildren<Collider>();
        
    }

    // Update is called once per frame
    public void ToggleRagdoll()
    {
        isRagdoll = !isRagdoll;
        animator.enabled = !isRagdoll;
        ToogleRigidbodies(isRagdoll);
        foreach( var c in ragdollColliders)
        {
            c.enabled = isRagdoll;
        }

        var components = GetComponents<MonoBehaviour>();
        foreach( var t in components )
        {
            t.enabled = !isRagdoll;
        }

    }

    public void AddForce()
    {
        ragdollHead.AddForce(transform.forward * 200f);
    }

    public void ToogleRigidbodies(bool state)
    {
        foreach(var rb in rigidBodies)
        {
            rb.isKinematic = !state;
        }
    }



}
