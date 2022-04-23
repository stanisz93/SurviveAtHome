using UnityEngine;
using System.Collections.Generic;
using System;

public class CraftItemProcessor : IItemProcessor {
    
    
    public Inventory inventory;
    public CraftItemProcessor(Inventory inventory)
    {
        this.inventory = inventory;
    }

    
    
    public bool CanProcess(GameObject item)
    {
        ICraftable trapable = item.GetComponent<ICraftable>();
        if (trapable != null)
            return true;
        else
            return false;
    }
    public void AddToInventory(GameObject item)
    {   

        
        ICraftable trapable = item.GetComponent<ICraftable>();
        CraftType trapType = trapable.GetCraftType();
        inventory.AddCraftItem(trapType, 1);
        foreach(KeyValuePair<string, int> res in trapable.GetRequirements())
        {
            ResourceType resType;
            Enum.TryParse(res.Key, out resType);
            inventory.SubstractResource(resType, res.Value);
        }
    }

    
    
}