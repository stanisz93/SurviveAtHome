using UnityEngine;

public class TrapItemProcessor : IItemProcessor {
    
    public bool CanProcess(Item item)
    {
        if (item.GetItemType().Equals(typeof(TrapType)))
            return true;
        else
            return false;
    }
    public void AddToInventory(Item item, Inventory inventory)
    {
        inventory.traps[item.GetItemName().ToString()] += item.amount;
    }
    
    
}