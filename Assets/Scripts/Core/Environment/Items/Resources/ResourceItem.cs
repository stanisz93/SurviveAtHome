using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class ResourceItem : Item{
    
    public void Awake() {
        itemType = typeof(ResourceType);
    }


    public override void OnTriggerEnter(Collider other) 
   {  
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>(); /// Here 
            itemPickuper.AddPotentialObject(this);
        }
    }
    public  override void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>();
            itemPickuper.RemovePotentialObject(this);
        }
    }
    public ResourceType GetResourceType() {return (ResourceType) enumName;}



}