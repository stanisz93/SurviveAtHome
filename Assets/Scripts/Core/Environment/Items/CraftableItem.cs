using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class CraftableItem : Item {

    [SerializeField]
    protected Dictionary<string, int> requiredResources;
    public Image uiItem; // every craftable item should be possible to craft in UI

    public abstract void Use(); // Describe how applied action affect this item

    public abstract void HoldAnimation(); // Describe how this method affect animation - probably as additional layer

    private void Awake() {
        
        itemType = typeof(TrapType);
    }   
}