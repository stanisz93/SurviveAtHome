using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public abstract class Item : MonoBehaviour
{
    // Start is called before the first frame update
   
    protected Type itemType;
    [SerializeField]
    abstract public int amount {get;  set; }
    protected string itemName;
    protected Enum enumName;

   public event Action<Item> OnPickup;
    public virtual Type GetItemType()
    {
        return itemType;
    }
    public virtual Enum GetItemName()
    {
        return enumName;
    }
    public virtual void DestroyAfterUse()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
   public abstract void OnTriggerEnter(Collider other); 
    public abstract void OnTriggerExit(Collider other);

    public void RunPickEvent()
    {
        OnPickup?.Invoke(this);
        OnPickup = null;
    }



}
