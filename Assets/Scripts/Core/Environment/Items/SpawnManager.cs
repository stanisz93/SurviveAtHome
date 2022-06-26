using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : MonoBehaviour
{

    public List<Transform> itemLocations;
    public CollectiblePopup collectiblePopup;
    public Color outlineColor;
    public float outlineWidth;
    public List<GameObject> pfItems;
    public Inventory inventory;
    // Start is called before the first frame update
    void Awake()
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

    public void AddOutlineToCollectObject(ICollectible collectible)
    {
        var outline = collectible.gameObject.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = new Color(0f, 1f, 0.757f, 1f);
        outline.OutlineWidth = 1.42f;
        outline.enabled = false;
    }

    void SpawnResource(GameObject itemPrefab, Transform spawnLocation)
    {
        GameObject itemObj = Instantiate(itemPrefab, spawnLocation);
        ICollectible collectItem = itemObj.GetComponent<ICollectible>();
        AddOutlineToCollectObject(collectItem);

        if (collectItem == null)
         Debug.LogError("Spoon prefab should have attached SpoonItem script to it!");
         collectItem.OnPickup += inventory.HandleResourcePickup;
         collectItem.OnPickup += collectiblePopup.PopUp;
    }

    // Update is called once per frame
}
