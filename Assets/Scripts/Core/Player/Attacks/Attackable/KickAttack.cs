using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickAttack : MonoBehaviour, IAttackable
{
    // Start is called before the first frame update
    
    public TriggerType triggerType {get 
            {return TriggerType.Melee;}}

    [SerializeField]
    private float _blockMovement = 0.8f;
    public float blockMovement{ get {return _blockMovement;} set{_blockMovement = value;}}

    [SerializeField]
    private float _releaseTriggerTime = 0.8f;

    public AttackType attackType { get{return AttackType.Kick;}}


    public float releaseTriggerTime{ get {return _releaseTriggerTime;} set{_releaseTriggerTime = value;}}

    [SerializeField]
    private float _distanceLeft = 0.2f;
    public float distanceLeft{ get {return _distanceLeft;} set{_distanceLeft = value;}}

    private string _animName;
    public string meleeAnimName{ get {return _animName;} set {_animName = value;}}

    private HitBonus hitBonus;
    void Start()
    {
        hitBonus = GameObject.FindWithTag("Player").gameObject.GetComponent<HitBonus>();
        
        
    }

    // Update is called once per frame
    public void ReleaseAttack()
    {
        meleeAnimName = hitBonus.GetBonusMode() == BonusMode.SuperKick ? "SuperKick" : "Kick";
    
    }
}
