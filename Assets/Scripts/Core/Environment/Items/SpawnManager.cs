using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public List<Transform> itemLocations;

    public Character character;
    public GameObject spoonPrefab;
    // Start is called before the first frame update
    private Inventory inventory;
    void Start()
    {
        inventory = character.GetInventory();
        
        foreach(Transform loc in itemLocations)
        {
            SpawnSpoon(loc);
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

    void SpawnSpoon(Transform spawnLocation)
    {
        GameObject spoonObject = Instantiate(spoonPrefab, spawnLocation);
        SpoonItem spoonItem = spoonObject.GetComponent<SpoonItem>();
        if (spoonItem == null)
         Debug.LogError("Spoon prefab should have attached SpoonItem script to it!");
         spoonItem.OnPickup += inventory.HandlePickup;
    }

    // Update is called once per frame
}
