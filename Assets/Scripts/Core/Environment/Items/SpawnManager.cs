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
            SpawnObject(pfItems[index], loc);
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

    void SpawnObject(GameObject itemPrefab, Transform spawnLocation)
    {
        GameObject itemObj = Instantiate(itemPrefab, spawnLocation);
        Item item = itemObj.GetComponent<Item>();

        if (item == null)
         Debug.LogError("Spoon prefab should have attached SpoonItem script to it!");
         item.OnPickup += inventory.HandlePickup;
    }

    // Update is called once per frame
}
