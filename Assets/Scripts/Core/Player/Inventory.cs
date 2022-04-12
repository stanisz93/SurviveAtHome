using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public enum ResourceType {Metal, Rope};
public enum TrapType {Wire};
public class Inventory
{
    // Start is called before the first frame update
    Dictionary<string, int> resources;

    public Inventory()
    {
        this.resources = Enum.GetNames(typeof(ResourceType))
                    .ToDictionary(t => t, t => 0);
    }

    // Update is called once per frame
    
    public void AddResource(ResourceType resourceType, int count )
    {
        this.resources[resourceType.ToString()] += count;
    }

    public void SubstractValue(ResourceType resourceType, int count )
    {
        this.resources[resourceType.ToString()] -= count;
        if (resources[resourceType.ToString()] < 0)
            this.resources[resourceType.ToString()] = 0;
    }
    public void HandlePickup(SpoonItem spoonItem)
    {
        this.AddResource(spoonItem.GetResourceType(), spoonItem.GetAmount());
        spoonItem.DestroyAfterUse();
    }
}
