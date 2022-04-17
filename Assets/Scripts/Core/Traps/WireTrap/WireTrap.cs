using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireTrap : CraftableItem, ITrapable {


    public override int amount 
    {
        get {return 1;}
        set {}
    }
    public ResourceType resourceType;
    private void Awake() {
        requiredResources = new Dictionary<string, int>(){{ResourceType.Metal.ToString(), 2}};
    }

    public void SetTrap()
    {
        Debug.Log("Setting trap mechanism-> A-B points etc");
    }

    public override void OnTriggerEnter(Collider other) 
   {  
        Debug.Log("A");
    }
    
   public override void OnTriggerExit(Collider other) 
   {  
        Debug.Log("A");
    }

    

    public override void Use()
    {
        Debug.Log("Use");
    }

    public override void HoldAnimation()
    {
        Debug.Log("HoldAnim");
    }

    
}