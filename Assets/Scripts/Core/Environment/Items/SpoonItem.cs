using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class SpoonItem : MonoBehaviour, ICollectible
{

   public  Action<ICollectible> OnPickup {get; set;}
    public int GetAmount()
    {
        return 1;
    }

    public void OnTriggerEnter(Collider other) 
   {  
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>(); /// Here 
            itemPickuper.AddPotentialObject(transform);
        }
    }
    public  void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>();
            itemPickuper.RemovePotentialObject(this.transform);
        }
    }

   public ResourceType GetResourceType()
    {
        return ResourceType.Metal;
    }

    public void Collect()
    {
        OnPickup?.Invoke(this);
    }

    public Sprite GetImage() {return null;}

    public void DestroyItem(float delay=0f, bool usingAnimation=false)
    { 
        Destroy(gameObject, delay);
    }

}
