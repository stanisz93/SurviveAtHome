using UnityEngine;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class StickItem : MonoBehaviour, IDefendable {
    
    public GameObject hand;
    public GameObject pfDestroyObject;
    public Transform posWhileAttack;
    public PlayerAnimatorEventController eventController;
    public PushTrigger pushTrigger;

    public Vector3 pickPosition;
    public Vector3 pickRotation;

    public Vector3 attackPosition;
    public Vector3 attackRotation;
    public Image icon; 


    private CharacterMovement characterMovement;
    private Collider interactCollider;

    public int maxEndurance = 200;
    private int endurance;
    [SerializeField]
    private int enduranceStep = 20;

    private void Awake() {
        endurance = maxEndurance;
        var plr = GameObject.FindWithTag("Player");
        characterMovement = plr.GetComponent<CharacterMovement>();
        interactCollider = GetComponent<Collider>();
        
    }
    public void AttachToPlayer()
    {
        GameObject plr = GameObject.FindWithTag("PlayerMesh");
        transform.parent = hand.transform;
        ChangeWeaponPositionToHold();
        characterMovement.SetHoldMode(HoldMode.WoddenStick);
        eventController.SetPushCollider(pushTrigger);
        pushTrigger.OnHit += ReduceEndurance;
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

    public void ChangeWeaponPositionToAttack()
    {
        transform.localPosition = attackPosition;
        transform.localEulerAngles = attackRotation;
    }
    public void ChangeWeaponPositionToHold()
    {
        // transform.DOLocalMove(pickPosition, duration);
        // transform.DORotate(pickRotation, .2f, RotateMode.LocalAxisAdd);
        //  transform.DOLocalRotate(pickRotation, duration);
        transform.localPosition = pickPosition;
        transform.localEulerAngles = pickRotation;
    }
    public void OnTriggerEnter(Collider other) 
   {  
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>(); /// Here 
            itemPickuper.AddPotentialObject(this.transform);
        }
    }
    public  void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>();
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
        pushTrigger.OnHit += action;
    }


    public Sprite GetImage()
    {
        return icon.sprite;
    }


}