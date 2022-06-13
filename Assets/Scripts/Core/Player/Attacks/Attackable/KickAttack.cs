using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickAttack : MonoBehaviour, IAttackable
{
    // Start is called before the first frame update
    
    public TriggerType triggerType {get 
            {return TriggerType.Melee;}}
    public float blockMovement{ get {return 0.8f;}}

    public float releaseTriggerTime{ get {return 0.8f;}}

    [SerializeField]
    private float _distanceLeft = 0.2f;
    public float distanceLeft{ get {return _distanceLeft;} set{_distanceLeft = value;}}

    private string _animName;
    public string animName{ get {return _animName;} set {_animName = value;}}

    private HitBonus hitBonus;
    void Start()
    {
        hitBonus = GameObject.FindWithTag("Player").gameObject.GetComponent<HitBonus>();
        
        
    }

    // Update is called once per frame
    public void ReleaseAttack()
    {
        animName = hitBonus.GetBonusMode() == BonusMode.SuperKick ? "SuperKick" : "Kick";
    
    }
}
