using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultKickAttack : MonoBehaviour, IAttackable
{
    // Start is called before the first frame update
    
    public TriggerType triggerType {get 
            {return TriggerType.Melee;}}
    [SerializeField]
    private float _blockMovement = 0.5f;
    public float blockMovement{ get {return _blockMovement;} set{_blockMovement = value;}}

    [SerializeField]
    private float _releaseTriggerTime = 0.5f;

    public AttackType attackType { get{return AttackType.KickWhileVault;}}


    public float releaseTriggerTime{ get {return _releaseTriggerTime;} set{_releaseTriggerTime = value;}}


    [SerializeField]
    private float _distanceLeft = 0.2f;
    public float distanceLeft{ get {return _distanceLeft;} set{_distanceLeft = value;}}

    private string _animName;
    public string meleeAnimName{ get {return "KickWhileVault";}}

    private HitBonus hitBonus;
    void Start()
    {
        hitBonus = GameObject.FindWithTag("Player").gameObject.GetComponent<HitBonus>();
        this.enabled = false;
        
    }

    // Update is called once per frame
    public void ReleaseAttack()
    {
        hitBonus.SetSuperKick();
    }
}
