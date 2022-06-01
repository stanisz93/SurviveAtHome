using UnityEngine;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DefendItem : MonoBehaviour {
    
    public PlayerAnimatorEventController eventController;
    public GameObject pfDestroyObject;

    public HoldMode holdMode;
    public AttackTrigger attackTrigger;

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
     private Action<DefendItem> OnDrop;
    
    public Action<DefendItem> OnHit;


    

    public int initialEndurance = 200;
    private int endurance;
    [SerializeField]
    private int enduranceStep = 20;
    private bool isCollected = false;
    private HitBonus bonus;
    private StressReceiver stressReceiver;
    private TrailRenderer trail;
    
    private void Awake() {
        stressReceiver = GameObject.FindWithTag("MainCamera").GetComponent<StressReceiver>();
        endurance = initialEndurance;
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
        OnPickup += weaponPlaceholder.AttachWeapon;
        OnDrop += weaponPlaceholder.RemoveWeapon;

        OnHit += bonus.IncreaseCounts;
        OnHit += stressReceiver.InduceStressByHit;
        OnHit += ReduceEndurance;

        
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
        eventController.SetAttackTriggerCollider(attackTrigger);
        interactCollider.enabled = false;
    }

    void BreakWeapon() {
        
        GameObject pfInstance = Instantiate(pfDestroyObject, transform.position, transform.rotation);  
        var rigidbodies = pfInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in rigidbodies)
            body.AddForce(UnityEngine.Random.insideUnitCircle.normalized * 400, ForceMode.Impulse);
        eventController.SetToDefaultPushTrigger();
        characterMovement.SetHoldMode(HoldMode.Default);
        weaponPlaceholder.RemoveWeapon(this);
        Destroy(gameObject);
        Destroy(pfInstance, 0.15f);
    }

    public void SetKinematic(bool state)
    {
        m_Rigidbody.isKinematic = state;
    }

    public int GetInitialEndurance()
    {
        return initialEndurance;
    }

    public int GetCurrentEndurance()
    {
        return endurance;
    }

    public void Drop(DefendItem defendItem)
    {
        DetachFromPlayer();
        physicsCollider.enabled = true;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.AddForce(GameObject.FindWithTag("PlayerMesh").transform.forward * 2f, ForceMode.Impulse); 
    }

    public void DetachFromPlayer()
    {
        // this could also be added to OnDrp
        eventController.SetToDefaultPushTrigger();
        characterMovement.SetHoldMode(HoldMode.Default);
        OnDrop?.Invoke(this);
        OnDrop = null;
        isCollected = false;
        interactCollider.enabled = true;
        OnHit -= ReduceEndurance;
        transform.parent = null;
    }

    public void PhysicFinishOfThrow(Vector3 forceDir)
    {
        physicsCollider.enabled = true;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.AddForce(forceDir, ForceMode.Impulse);
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

    public void ReduceEndurance(DefendItem idefendable)
    {
        endurance -= enduranceStep;
        weaponPlaceholder.UpdateEndurance(endurance); //remember to remove it when drop
    
        if(endurance <= 0)
        {
            BreakWeapon();
        }

    }



    public Sprite GetImage()
    {
        return icon.sprite;
    }


}