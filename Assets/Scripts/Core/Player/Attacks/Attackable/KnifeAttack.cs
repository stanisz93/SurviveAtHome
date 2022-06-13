using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAttack : MonoBehaviour, IAttackable
{
    // Start is called before the first frame update
    
    public TriggerType triggerType {get 
            {return TriggerType.Melee;}}
    public float blockMovement{ get {return 0.4f;}}

    public float releaseTriggerTime{ get {return 0.4f;}}

    public string animName{ get {return "Stab";}}

    [SerializeField]
    private float _distanceLeft = 0.2f;
    public float distanceLeft{ get {return _distanceLeft;} set{_distanceLeft = value;}}

    

    private DefendItem defendItem;
    void Start()
    {
        defendItem = GetComponentInParent<DefendItem>();
        
    }

    // Update is called once per frame
    public void ReleaseAttack()
    {
       defendItem.ChangeWeaponPositionToAttack();
    }
}
