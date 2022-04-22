    using UnityEngine;
    using System;
    public interface IItemProcessor
    {
    bool CanProcess(GameObject item);

    void AddToInventory(GameObject item);

    }