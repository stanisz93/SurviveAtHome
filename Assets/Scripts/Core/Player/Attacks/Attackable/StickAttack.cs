using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickAttack : MonoBehaviour, IAttackable
{
    // Start is called before the first frame update
    
    public TriggerType triggerType {get 
            {return TriggerType.Melee;}}
    public float blockMovement{ get {return 0.4f;}}

    public float releaseTriggerTime{ get {return 0.5f;}}

    [SerializeField]
    private float _distanceLeft = 0.2f;
    public float distanceLeft{ get {return _distanceLeft;} set{_distanceLeft = value;}}


    public string animName{ get {return "PushStick";}}

    private DefendItem defendItem;
    void Start()
    {
        defendItem = GetComponentInParent<DefendItem>();
        
    }

    // Update is called once per frame
    public void ReleaseAttack()
    {
        // attackTriggerManager.GetCurrentAttackTrigger().SetTriggerType(TriggerType.Melee);
        // DefendItem defendItem = GetComponentInChildren<DefendItem>();
        defendItem.ChangeWeaponPositionToAttack();
        // character.SpeedBeforeKick = character.GetVelocityMagnitude();
        // StartCoroutine(BlockMovementSeconds(0.4f));
        // StartCoroutine(ReleaseTriggerAfterSeconds(0.5f));
    
    }
}
