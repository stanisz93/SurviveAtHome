using UnityEngine;
using UnityEngine.UI;
using System.Linq;
[RequireComponent(typeof(ICraftable))]
public class ItemTile : InventoryTile 
{
    public Text currentAmount;
    public Text requirement;
    // [SerializeField]
    // private CraftableItem craftableItemPrefab; in the future
    //when I want to connect actual object to it

    // private void Awake() {
    //     image = craftableItemPrefab.uiItem;
    //     Deactivate();
    // }
    private Inventory inventory;
    private ICraftable craftable;
    private string maxAmount = "10";

    private void Awake() {

        craftable = GetComponent<ICraftable>();
        inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        image = transform.Find("Icon").GetComponent<Image>();
        SetRequirement();
        RestartAmount();
        base.Awake();
    } 

    void SetAmount(int amount)
    {
        currentAmount.text = $"{amount.ToString()} / {maxAmount}";
    }


    void RestartAmount()
    {
        SetAmount(0);
    }

    void SetRequirement()
    {
        var d = craftable.GetRequirements();
        requirement.text = craftable.GetRequirements().Take(1).Select(d => d.Value.ToString()).First();
    }


    public void TryToCraft() //here method to craft specific item
    {
        bool success;
        inventory.AddItem(this.gameObject, out success);
        if(success)
            {
                SetAmount(inventory.crafts[craftable.GetCraftType().ToString()]);
            }
        Debug.Log("Trying crafting");
    }




}