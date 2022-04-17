    public interface IItemProcessor
    {
    bool CanProcess(Item item);

    void AddToInventory(Item item, Inventory inventory);
    }