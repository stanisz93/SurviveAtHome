using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThrowableKnife : MonoBehaviour
{

    
    public AnimationCurve forceCurve;
    public float forceTorqueThrow = 100f;

    public float ThrowSlowDown = 1.0f;
    
    private DefendItem item;
    private AttackTrigger attackTrigger;

    private TrailRenderer trail;


    private StressReceiver stressReceiver;
    private Rigidbody rb;

    // Start is called before the first frame update
    
    void Start()
    {
        attackTrigger = GetComponentInChildren<AttackTrigger>();
        item = GetComponentInParent<DefendItem>();
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
        stressReceiver = Camera.main.GetComponent<StressReceiver>();
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    
    private void OnCollisionEnter(Collision other) {
        trail.enabled = false;
        Time.timeScale = 1f;
        IThrowStickable stickable = other.gameObject.GetComponent<IThrowStickable>();
        if (stickable != null)
            {
                Debug.Log($"Parented to {other}");
                item.SetParentObj(other.transform);
                item.SetKinematic(true);
                item.physicsCollider.enabled = false;
                Opponent opponent = other.gameObject.GetComponentInParent<Opponent>();
                if(opponent != null)
                {
                    attackTrigger.SetDamageType(DamageType.Killing);
                    attackTrigger.InduceTrigger(opponent.gameObject);
                }
            }
            
                // if(other.gameObject.GetComponentInParent<Opponent>())
                //     stressReceiver.InduceStress(2f);
                
    }

    public void TurnOnTrail()
    {
        trail.enabled = true;
    }


    public void Throw(Vector3 targetThrowPos, float distance)
    {
            Vector3 itemPos = item.transform.position;
            Vector3 targetPos = itemPos + (targetThrowPos - itemPos);
            Time.timeScale = ThrowSlowDown;
            attackTrigger.SetHitType(HitType.OneVictim);
            // attackTrigger.GetComponent<Collider>().enabled = true;
            TurnOnTrail();
            item.DetachFromPlayer();
            item.transform.localRotation = Quaternion.FromToRotation(Vector3.right, -(targetThrowPos - item.transform.position).normalized);
            rb.isKinematic = false;
            item.interactCollider.enabled = true;
            
            // hasCollideWhileThrow = false;
            item.physicsCollider.enabled = true;
            Vector3 itemModPos = new Vector3(item.transform.position.x, targetThrowPos.y, item.transform.position.z);
            Vector3 force = (targetThrowPos - itemModPos).normalized * forceCurve.Evaluate(distance);
            Debug.Log($"Force: {force}");
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(transform.forward * forceTorqueThrow);
    }
    


}
