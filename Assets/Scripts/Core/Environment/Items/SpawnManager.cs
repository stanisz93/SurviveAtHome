using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : MonoBehaviour
{

    public List<Transform> itemLocations;

    public List<GameObject> pfItems;
    public Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        var random = new System.Random(1);
        foreach(Transform loc in itemLocations)
        {
            int index = random.Next(pfItems.Count);
            SpawnResource(pfItems[index], loc);
        }
    }


    // Item GetRandomItem()
    // {
    //     return
    // }

    // void SpawnItem(Item item, Transform position)
    // {
    //     Instantiate(item, position);
    //     spoon.OnPickup += inventory.HandlePickup;
    // }

    void SpawnResource(GameObject itemPrefab, Transform spawnLocation)
    {
        GameObject itemObj = Instantiate(itemPrefab, spawnLocation);
        ICollectible collectItem = itemObj.GetComponent<ICollectible>();

        if (collectItem == null)
         Debug.LogError("Spoon prefab should have attached SpoonItem script to it!");
         collectItem.OnPickup += inventory.HandleResourcePickup;
    }

    // Update is called once per frame
}
