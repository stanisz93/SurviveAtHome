using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public enum ResourceType {Metal, Cloth};
public enum CraftType {Wire, SoundNoiser};
public enum CollectibleType {Spoon};


public enum ItemType {Collectible, Craftable}

public class Inventory : MonoBehaviour 

{
    // Start is called before the first frame update

    private List<IItemProcessor> itemProcessors;
    public Dictionary<string, int> resources;
    public Dictionary<string, int> crafts;
    private InventoryUI inventoryUI;
    


    public void Awake()
    {
        this.resources = Enum.GetNames(typeof(ResourceType))
                    .ToDictionary(t => t, t => 0);
        this.crafts = Enum.GetNames(typeof(CraftType))
                    .ToDictionary(t => t, t => 0);
        itemProcessors = new List<IItemProcessor>();
        itemProcessors.Add(new ResourceItemProcessor(this));
        itemProcessors.Add(new CraftItemProcessor(this));
        inventoryUI = GetComponentInChildren<InventoryUI>();

    }

    // Update is called once per frame
    
    public bool SatisfyRequirement(Dictionary<string, int> requiredRes)
    {
        foreach(KeyValuePair<string, int> res in requiredRes)
        {
            if(this.resources[res.Key] < requiredRes[res.Key])
            return false;   
        }
        return true;
    }

    public void AddItem(GameObject item)
    {
        foreach(IItemProcessor p in itemProcessors)
        {
            if(p.CanProcess(item))
            {
                p.AddToInventory(item);
            }
        }
    }




    public void AddResource(ResourceType resType, int amount)
    {
        resources[resType.ToString()] += amount;
        inventoryUI.UpdateResourcesUI(resType, resources[resType.ToString()]);
    }

    public void SubstractResource(ResourceType resType, int amount)
    {
        resources[resType.ToString()] -= amount;
        inventoryUI.UpdateResourcesUI(resType, resources[resType.ToString()]);
    }

    public void AddCraftItem(CraftType trapType, int amount)
    {
        crafts[trapType.ToString()] += amount;
        // here should be some method from ui to update value in ui for trap
    }

    public void SubstractCraftItem(CraftType trapType, int amount)
    {
        crafts[trapType.ToString()] -= amount;
        // here should be some method from ui to update value in ui for trap
    }


    public void HandleResourcePickup(ICollectible item)
    {
        AddItem(item.gameObject);
        item.OnDestroy();
    }

    



    // public void SubstractItem(T anyItemType, int count)
    // {
    //     if (!typeof(T).IsEnum)
    //         throw new ArgumentException("anyItemType must be enum type");
        
    //     if (ResourceType.IsDefined(typeof(ResourceType), anyItemType))
    //     {
    //         this.resources[anyItemType.ToString()] -= count;
    //         if (resources[anyItemType.ToString()] < 0)
    //         this.resources[anyItemType.ToString()] = 0;
    //     }
    //     else if (TrapType.IsDefined(typeof(TrapType), anyItemType))
    //     {   
    //         this.traps[anyItemType.ToString()] -= count;
    //         if (resources[anyItemType.ToString()] < 0)
    //         this.resources[anyItemType.ToString()] = 0;
    //     }
    //     else if (OtherCraftItems.IsDefined(typeof(OtherCraftItems), anyItemType))
    //     {
    //             this.other[anyItemType.ToString()] -= count;
    //             if (resources[anyItemType.ToString()] < 0)
    //             this.resources[anyItemType.ToString()] = 0;
                
    //     }
    // }


}
