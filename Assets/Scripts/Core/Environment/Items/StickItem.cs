using UnityEngine;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class StickItem : MonoBehaviour, IDefendable {
    
    public GameObject hand;
    public GameObject pfDestroyObject;
    public PushTrigger pushTrigger;

    public Vector3 pickPosition;
    public Image icon; 

    public Vector3 pickRotation;
    private CharacterMovement characterMovement;
    private PlayerTriggers plrT;
    private Collider interactCollider;

    public int maxEndurance = 200;
    private int endurance;
    [SerializeField]
    private int enduranceStep = 20;

    private void Awake() {
        endurance = maxEndurance;
        var plr = GameObject.FindWithTag("Player");
        characterMovement = plr.GetComponent<CharacterMovement>();
        plrT = plr.GetComponent<PlayerTriggers>();
        interactCollider = GetComponent<Collider>();
        
    }
    public void AttachToPlayer()
    {
        GameObject plr = GameObject.FindWithTag("PlayerMesh");
        transform.parent = hand.transform;
        transform.localPosition = GetPickPosition();
        transform.localEulerAngles = GetPickRotation();
        characterMovement.SetHoldMode(HoldMode.WoddenStick);
        plrT.SetPushTrigger(pushTrigger);
        pushTrigger.OnHit += ReduceEndurance;
        interactCollider.enabled = false;
    }

    void BreakWeapon() {
        
        GameObject pfInstance = Instantiate(pfDestroyObject, transform.position, transform.rotation);  
        var rigidbodies = pfInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in rigidbodies)
            body.AddForce(UnityEngine.Random.insideUnitCircle.normalized * 400, ForceMode.Impulse);
        plrT.SetPushTrigger(plrT.defaultPushTrigger);
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


    public Vector3 GetPickPosition()
    {
        return pickPosition;
    }

    public Sprite GetImage()
    {
        return icon.sprite;
    }

    public Vector3 GetPickRotation()
    {
        return pickRotation;
    }

}