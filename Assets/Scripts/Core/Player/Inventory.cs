using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public enum ResourceType {Metal, Cloth};
public enum TrapType {Wire};
public enum OtherCraftItems {SoundNoiser};

public class Inventory : MonoBehaviour 

{
    // Start is called before the first frame update
    public InventoryUI ui;
    private List<IItemProcessor> itemProcessors;
    public Dictionary<string, int> resources;
    public Dictionary<string, int> traps;
    public Dictionary<string, int> otherCrafts;

    


    public void Awake()
    {
        this.resources = Enum.GetNames(typeof(ResourceType))
                    .ToDictionary(t => t, t => 0);
        this.traps = Enum.GetNames(typeof(TrapType))
                    .ToDictionary(t => t, t => 0);
        this.otherCrafts = Enum.GetNames(typeof(OtherCraftItems))
                    .ToDictionary(t => t, t => 0);
        itemProcessors = new List<IItemProcessor>();
        itemProcessors.Add(new ResourceItemProcessor());
        itemProcessors.Add(new TrapItemProcessor());

    }

    // Update is called once per frame
    
    public void AddItem(Item item)
    {
        foreach(var p in itemProcessors)
        {
            if(p.CanProcess(item))
            {
                p.AddToInventory(item, this);
            }
        }
    }

    public void HandlePickup(Item item)
    {
        this.AddItem(item);
        item.DestroyAfterUse();
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
