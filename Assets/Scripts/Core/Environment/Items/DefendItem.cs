using UnityEngine;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DefendItem : MonoBehaviour, ICollectible{
    
    public PlayerAnimatorEventController eventController;
    public GameObject pfDestroyObject;

    public WeaponType weaponType;
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
    
    public Action<ICollectible> OnPickup {get; set;}
    private Action OnDetachFromPlayer;

    

    public int initialEndurance = 200;
    private int endurance;
    [SerializeField]
    private int enduranceStep = 20;
    private bool isCollected = false;
    private HitBonus bonus;
    private TrailRenderer trail;
    
    private void Awake() {
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
        OnDetachFromPlayer += weaponPlaceholder.RemoveWeapon;

        
    }

    public ResourceType GetResourceType() {return ResourceType.None;}

    public int GetAmount() { return 0;}
    public void Collect()
    {
        OnPickup?.Invoke(this);
        isCollected = true;
        physicsCollider.enabled = false;
        m_Rigidbody.isKinematic = true;
        transform.parent = hand;
        ChangeWeaponPositionToHold();
        characterMovement.SetHoldMode(weaponType);
        eventController.SetAttackTriggerCollider(attackTrigger);
        interactCollider.enabled = false;
    }

    void InvokeDestroyObjectAnimation(float destroyDelay=0.15f)
    {
        GameObject pfInstance = Instantiate(pfDestroyObject, transform.position, transform.rotation);  
        var rigidbodies = pfInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in rigidbodies)
            body.AddForce(UnityEngine.Random.insideUnitCircle.normalized * 400, ForceMode.Impulse);
        Destroy(pfInstance, destroyDelay);
    }

    public void DestroyItem(float delay=0f, bool usingAnimation=true) {
        
        if(usingAnimation)
            InvokeDestroyObjectAnimation();
        eventController.SetToDefaultAttackTrigger();
        characterMovement.SetHoldMode(WeaponType.None);
        weaponPlaceholder.RemoveWeapon();
        Destroy(gameObject);

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

    public void Drop()
    {//Difference from detach from Player is that it is a part
    // that is happening while swamping weapon, or getin rid of it
        DetachFromPlayer();
        physicsCollider.enabled = true;
        m_Rigidbody.isKinematic = false;
        interactCollider.enabled = true;

        m_Rigidbody.AddForce(GameObject.FindWithTag("PlayerMesh").transform.forward * 2f, ForceMode.Impulse); 
    } 

    public void DetachFromPlayer()
    {
        OnDetachFromPlayer?.Invoke();
        // this could also be added to OnDrp
        eventController.SetToDefaultAttackTrigger();
        characterMovement.SetHoldMode(WeaponType.None);
        isCollected = false;
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

    public void ReduceEndurance(AttackTrigger attackTrigger)
    {
        endurance -= enduranceStep * (int) attackTrigger.GetEnduranceMultiplier();
        if(weaponPlaceholder.isItemCurrentOne(this))
            weaponPlaceholder.UpdateEndurance(endurance); //remember to remove it when drop
    
        if(endurance <= 0)
        {
            DestroyItem();
        }

    }



    public Sprite GetImage()
    {
        return icon.sprite;
    }

    public void SetParentObj(Transform parent)
    {
        transform.parent = parent;
         
    }


}