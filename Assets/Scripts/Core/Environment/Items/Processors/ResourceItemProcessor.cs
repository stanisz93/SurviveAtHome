using UnityEngine;
using System;

public class ResourceItemProcessor : IItemProcessor {
    
    private Inventory inventory;

    public ResourceItemProcessor(Inventory inventory)
    {
        this.inventory = inventory;
    }


    public bool CanProcess(GameObject item)
    {
        if (item.GetComponent<ICollectible>() != null)
            return true;
        else
            return false;
    }

    public void AddToInventory(GameObject item)
    {
        ICollectible collectible = item.GetComponent<ICollectible>();
        ResourceType resType = collectible.GetResourceType();
        this.inventory.AddResource(resType, collectible.GetAmount());
    }



}