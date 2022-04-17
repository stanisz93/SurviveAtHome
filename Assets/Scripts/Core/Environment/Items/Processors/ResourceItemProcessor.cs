using UnityEngine;

public class ResourceItemProcessor : IItemProcessor {
    
    public bool CanProcess(Item item)
    {
        if (item.GetItemType().Equals(typeof(ResourceType)))
            return true;
        else
            return false;
    }

    public void AddToInventory(Item item, Inventory inventory)
    {
        inventory.resources[item.GetItemName().ToString()] += item.amount;

        ResourceType addedResource = (ResourceType) item.GetItemName();
        
        if (addedResource == ResourceType.Metal)
        {
            inventory.ui.UpdateMetalResource(inventory.resources[addedResource.ToString()]);
        }
        else if(addedResource == ResourceType.Cloth)
        {
            inventory.ui.UpdateClothResource(inventory.resources[addedResource.ToString()]);
        }
        
    }

}