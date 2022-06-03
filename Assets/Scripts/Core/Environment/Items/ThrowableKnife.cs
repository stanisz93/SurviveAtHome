using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class ThrowableKnife : MonoBehaviour
{

    
    public float degressPerUnit = 2f;
    public AnimationCurve throwAcceleration;
    public float throwVelocity = 10f;
    public float forceThrow = 10f;

    public float ThrowSlowDown = 1.0f;
    
    private Sequence throwSequence;
    private DefendItem item;
    private AttackTrigger attackTrigger;
    private bool hasCollideWhileThrow = false;

    private TrailRenderer trail;

    private bool isKillingThrow = false;
    private bool alreadParented = false;

    private StressReceiver stressReceiver;

    // Start is called before the first frame update
    
    void Start()
    {
        attackTrigger = GetComponentInChildren<AttackTrigger>();
        item = GetComponentInParent<DefendItem>();
        trail = GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
        stressReceiver = Camera.main.GetComponent<StressReceiver>();
        
    }

    // Update is called once per frame
    
    private void OnCollisionEnter(Collision other) {
        trail.enabled = false;
        IThrowStickable stickable = other.gameObject.GetComponent<IThrowStickable>();
        if (stickable != null)
            {
                if(throwSequence.IsActive())
                    throwSequence.Kill();
                hasCollideWhileThrow = true;
                item.SetKinematic(true);
                if(!alreadParented)
                    {
                        item.SetParentObj(other.transform);
                        alreadParented = true;
                    }
                item.physicsCollider.enabled = false;
                // if(other.gameObject.GetComponentInParent<Opponent>())
                //     stressReceiver.InduceStress(2f);
                
            }
    }

    public void TurnOnTrail()
    {
        trail.enabled = true;
    }


    float GetPlannedTimeOfThrow(Vector3 targetPos)
    {
        return(targetPos - item.transform.position).magnitude / throwVelocity;
    }

    float FinalRotationInDegree(Vector3 targetPos)
    {
        return degressPerUnit * (targetPos - item.transform.position).magnitude;
    }

    void TurnOffKillingThrow()
    {
        isKillingThrow = false;
    }

    public void Throw(Vector3 targetThrowPos)
    {
            Vector3 itemPos = item.transform.position;
            Vector3 targetPos = itemPos + (targetThrowPos - itemPos);
            Time.timeScale = ThrowSlowDown;
            TurnOnTrail();
            item.DetachFromPlayer();
            
            item.transform.localRotation = Quaternion.FromToRotation(Vector3.right, -(targetThrowPos - item.transform.position).normalized);
        

            if(throwSequence.IsActive())
                throwSequence.Kill();
            
            ApplyThrowSequence(targetThrowPos);
            // throwSequence.AppendInterval(2f);
            // throwSequence.AppendCallback(() => {defendItem.transform.parent = throwingHand.transform;
            //     defendItem.transform.localPosition = initialPos;
            //     defendItem.transform.localEulerAngles = initialRot;
            //     defendItem.SetKinematic(true);
            //     defendItem.physicsCollider.enabled = false;});
            
    }

    public void ApplyThrowSequence(Vector3 targetThrowPos)
    {
        attackTrigger.GetComponent<Collider>().enabled = true;
        hasCollideWhileThrow = false;
        item.physicsCollider.enabled = true;
        throwSequence = DOTween.Sequence();
        Vector3 itemModPos = new Vector3(item.transform.position.x, targetThrowPos.y, item.transform.position.z);
        Vector3 force = (targetThrowPos - itemModPos).normalized * forceThrow;
        //calcualte number or rotation base on distance
        float throwTime = GetPlannedTimeOfThrow(targetThrowPos);
        throwSequence.Append(item.transform.DOMove(targetThrowPos, throwTime).SetEase(throwAcceleration));
        
        float finalRotate = FinalRotationInDegree(targetThrowPos);
        throwSequence.Join(item.transform.DOLocalRotate(new Vector3(0, 0, finalRotate), throwTime, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear));
        throwSequence.AppendCallback(() =>  Time.timeScale = 1f);

        throwSequence.AppendCallback(() => 
        {   
            if(!hasCollideWhileThrow)
                GetComponentInParent<DefendItem>().PhysicFinishOfThrow(force);
        });
        throwSequence.OnComplete(() => {  
    Debug.Log("Done"); 
});
    }


}
