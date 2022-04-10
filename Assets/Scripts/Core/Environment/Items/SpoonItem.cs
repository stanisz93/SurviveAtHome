using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class SpoonItem : MonoBehaviour
{
    // Start is called before the first frame update
    public event Action<SpoonItem> OnPickup;

    [SerializeField]
    private Vector3 globalPosition;
    private ResourceType resourceType = ResourceType.Metal;
    private int amount = 1;

    private void Start() {
        globalPosition = transform.position;
    }

   public int GetAmount() { return amount;}
   public ResourceType GetResourceType() {return resourceType;}

    public void DestroyAfterUse()
    {
        Destroy(gameObject);
    }

   private void OnTriggerEnter(Collider other) 
   {  
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>(); /// Here 
            itemPickuper.AddPotentialObject(this);
            // You should add to potential objrcts

        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Player")
        {
            var itemPickuper = other.gameObject.GetComponent<ItemPickupManager>();
            itemPickuper.RemovePotentialObject(this);
        }
        // // biggest.RunPickEvent();
    }

    public void RunPickEvent()
    {
        OnPickup?.Invoke(this);
        OnPickup = null;
    }
    public float GetRelativeDirection(Transform obj)
    {
        Vector3 dir = (transform.position - obj.position).normalized;
        return Vector3.Dot(dir, obj.forward);   
    }

    // private void Update() {
    //     globalPosition = transform.position;
    // }
}
