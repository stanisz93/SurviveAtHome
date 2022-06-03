using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject Zombie;

    public Rigidbody ragdollHead;
    public Rigidbody hips;

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
    
    public IEnumerator ToggleRagdollAfter(float delay, Vector3 force, float rotateForce)
    {
        yield return new WaitForSeconds(delay);
        ToggleRagdoll(force, rotateForce);
    }
    public void ToggleRagdoll(Vector3 force, float rotateForce)
    {
        isRagdoll = !isRagdoll;
        animator.enabled = !isRagdoll;
        GetComponent<Collider>().enabled = !isRagdoll;
        ToogleRigidbodies(isRagdoll);
        foreach( var c in ragdollColliders)
        {
            c.enabled = isRagdoll;
        }

        var components = GetComponents<MonoBehaviour>();
        foreach( var t in components)
        {
            t.enabled = !isRagdoll;
        }
        if (force != Vector3.zero)
            AddForce(force);

        if (rotateForce != 0.0f)
            AddRotationForce(rotateForce);
        

    }

    public void AddRotationForce(float rotateForce)
    {
        hips.AddTorque(Vector3.up * rotateForce, ForceMode.Impulse);
        // } 
        // foreach(var rb in rigidBodies)
        // {
        //     rb.AddTorque(ragdollHead.transform.up * rotateForce, ForceMode.Impulse);
        // } 
    }

    public void AddForce(Vector3 force)
    {
        // ragdollHead.AddForce(force * 1000f);
        foreach(var rb in rigidBodies)
        {
            rb.AddForce(force * 1000f);
        }
    }

    public void ToogleRigidbodies(bool state)
    {
        foreach(var rb in rigidBodies)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = !state;
        }
    }



}
