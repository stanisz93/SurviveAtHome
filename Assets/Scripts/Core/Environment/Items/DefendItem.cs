using UnityEngine;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DefendItem : MonoBehaviour, IDefendable {
    
    public PlayerAnimatorEventController eventController;
    public GameObject pfDestroyObject;

    public HoldMode holdMode;
    public AttackTrigger PushEffectTrigger;

    public Collider physicsCollider;

    public Vector3 pickPosition;
    public Vector3 pickRotation;

    public Vector3 attackPosition;
    public Vector3 attackRotation;
    public Image icon; 

    private Transform hand;


    private CharacterMovement characterMovement;
    private Collider interactCollider;
    private Rigidbody m_Rigidbody;
    private WeaponPlaceholder weaponPlaceholder;
    
    private Action<DefendItem> OnPickup;
    

    public int maxEndurance = 200;
    private int endurance;
    [SerializeField]
    private int enduranceStep = 20;
    private bool isCollected = false;
    private HitBonus bonus;
    
    private void Awake() {
        endurance = maxEndurance;
        var plr = GameObject.FindWithTag("Player");
        bonus = plr.GetComponent<HitBonus>();
        if(plr != null)
            {
                hand = plr.GetComponent<Character>().leftHand;
                characterMovement = plr.GetComponent<CharacterMovement>();
            }
        interactCollider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();
        weaponPlaceholder = plr.GetComponent<WeaponPlaceholder>();
        OnPickup += weaponPlaceholder.SetDefendable;
        
    }
    public void Collect()
    {
        isCollected = true;
        OnPickup?.Invoke(this);
        GameObject plr = GameObject.FindWithTag("Player");
        physicsCollider.enabled = false;
        m_Rigidbody.isKinematic = true;
        transform.parent = hand;
        ChangeWeaponPositionToHold();
        characterMovement.SetHoldMode(holdMode);
        eventController.SetAttackTriggerCollider(PushEffectTrigger);
        PushEffectTrigger.OnHit += ReduceEndurance;
        PushEffectTrigger.OnHit += weaponPlaceholder.UpdateEndurance; //remember to remove it when drop

        interactCollider.enabled = false;
    }

    void BreakWeapon() {
        
        GameObject pfInstance = Instantiate(pfDestroyObject, transform.position, transform.rotation);  
        var rigidbodies = pfInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in rigidbodies)
            body.AddForce(UnityEngine.Random.insideUnitCircle.normalized * 400, ForceMode.Impulse);
        eventController.SetToDefaultPushTrigger();
        characterMovement.SetHoldMode(HoldMode.Default);
        Destroy(gameObject);
        Destroy(pfInstance, 0.15f);
    }

    public int GetMaxEndurance()
    {
        return maxEndurance;
    }

    public int GetCurrentEndurance()
    {
        return endurance;
    }

    public void Drop()
    {
        isCollected = false;
        physicsCollider.enabled = true;
        m_Rigidbody.isKinematic = false;
        interactCollider.enabled = true;
        PushEffectTrigger.OnHit -= ReduceEndurance;
        PushEffectTrigger.OnHit -= weaponPlaceholder.UpdateEndurance;
        m_Rigidbody.AddForce(GameObject.FindWithTag("PlayerMesh").transform.forward * 2f, ForceMode.Impulse); 
        transform.parent = null;


    }

    public void ChangeWeaponPositionToAttack()
    {
        transform.localPosition = attackPosition;
        transform.localEulerAngles = attackRotation;
    }
    public void ChangeWeaponPositionToHold()
    {
        transform.localPosition = pickPosition;
        transform.localEulerAngles = pickRotation;
    }
    public void OnTriggerEnter(Collider other) 
   {  
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>(); /// Here 
            if(!isCollected)
                itemPickuper.AddPotentialObject(this.transform);
        }
    }
    public  void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>();
            if(!isCollected)
                itemPickuper.RemovePotentialObject(this.transform);
        }
    }

    public void ReduceEndurance()
    {
        endurance -= enduranceStep;
        if(endurance <= 0)
        {
            BreakWeapon();
        }
    }

    public void AddActionOnHit(Action action)
    {
        PushEffectTrigger.OnHit += action;
    }


    public Sprite GetImage()
    {
        return icon.sprite;
    }


}