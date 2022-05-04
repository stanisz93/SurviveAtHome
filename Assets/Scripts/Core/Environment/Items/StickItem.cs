using UnityEngine;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class StickItem : MonoBehaviour, IDefendable {
    
    public GameObject hand;

    public Vector3 pickPosition;
    public PushTrigger pushTrigger;

    public Image icon; 

    public Vector3 pickRotation;
    public Action<IDefendable> OnPickup {get; set;}
    private CharacterMovement characterMovement;
    private PlayerTriggers plrT;
    private Collider interactCollider;

    private void Awake() {
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
        interactCollider.enabled = false;
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