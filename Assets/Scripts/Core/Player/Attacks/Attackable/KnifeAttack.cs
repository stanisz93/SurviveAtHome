using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAttack : MonoBehaviour, IAttackable, IThrowable
{
    // Start is called before the first frame update
    
    public TriggerType triggerType {get 
            {return TriggerType.Melee;}}
    public float blockMovement{ get {return 0.4f;}}

    public float releaseTriggerTime{ get {return 0.4f;}}

    public string meleeAnimName { get {return "Stab";}}

    public string distantAnimName { get {return "Throw";}}

    public AnimationCurve forceCurve;
    public TrailRenderer trail;
    
    [SerializeField]
    private float forceTorqueThrow = 100f;

    [SerializeField]
    private float ThrowSlowDown = 1.0f;


    [SerializeField]
    private float _distanceLeft = 0.2f;
    public float distanceLeft{ get {return _distanceLeft;} set{_distanceLeft = value;}}


    private DefendItem defendItem;

    private AttackTrigger attackTrigger;
    private StressReceiver stressReceiver;
    private Rigidbody rb;

    private PlayerTriggers playerTriggers;

    private float distance; 
    private Vector3 targetThrowPos;

    void Start()
    {
        attackTrigger = GetComponent<AttackTrigger>();
        defendItem = GetComponent<DefendItem>();
        trail.enabled = false;
        stressReceiver = Camera.main.GetComponent<StressReceiver>();
        rb = GetComponentInParent<Rigidbody>();
        playerTriggers = GameObject.FindWithTag("Player").gameObject.GetComponent<PlayerTriggers>();
    }


    private void OnCollisionEnter(Collision other) {
        trail.enabled = false;
        Time.timeScale = 1f;
        defendItem.interactCollider.enabled = true;
        IThrowStickable stickable = other.gameObject.GetComponent<IThrowStickable>();
        if (stickable != null)
            {
                Debug.Log($"Parented to {other}");
                defendItem.SetParentObj(other.transform);
                defendItem.SetKinematic(true);
                defendItem.physicsCollider.enabled = false;
                Opponent opponent = other.gameObject.GetComponentInParent<Opponent>();
                if(opponent != null)
                {
                    attackTrigger.SetDamageType(DamageType.Killing);
                    attackTrigger.InduceTrigger(opponent);
                }
            }
                
    }

    void SetThrowTarget(Vector3 target)
    {
        targetThrowPos = target;

    }

    void SetDistance(float throwDistance)
    {
        distance = throwDistance; 
    }


    public void TurnOnTrail()
    {
        trail.enabled = true;
    }


    public void Throw()
    {

            Vector3 itemPos = defendItem.transform.position;
            Vector3 targetPos = itemPos + (targetThrowPos - itemPos);
            Time.timeScale = ThrowSlowDown;
            // attackTrigger.GetComponent<Collider>().enabled = true;
            TurnOnTrail();
            defendItem.DetachFromPlayer();
            defendItem.transform.localRotation = Quaternion.FromToRotation(Vector3.right, -(targetThrowPos - defendItem.transform.position).normalized);
            rb.isKinematic = false;
            
            // hasCollideWhileThrow = false;
            defendItem.physicsCollider.enabled = true;
            Vector3 itemModPos = new Vector3(defendItem.transform.position.x, targetThrowPos.y, defendItem.transform.position.z);
            Vector3 force = (targetThrowPos - itemModPos).normalized * forceCurve.Evaluate(distance);
            Debug.Log($"Force: {force}");
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(transform.forward * forceTorqueThrow);
    }

    // Update is called once per frame
    public void ReleaseAttack()
    {
       defendItem.ChangeWeaponPositionToAttack();
    }

    public void ReleaseThrow()
    {
        SetThrowTarget(playerTriggers.GetThrowTargetPos());
        SetDistance(playerTriggers.currentThrowDistance); 
    }
}
