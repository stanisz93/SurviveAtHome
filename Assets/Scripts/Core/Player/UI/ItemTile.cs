using UnityEngine;
using UnityEngine.UI;
public class ItemTile : InventoryTile 
{
    private Text currentAmountText;
    // [SerializeField]
    // private CraftableItem craftableItemPrefab; in the future
    //when I want to connect actual object to it

    // private void Awake() {
    //     image = craftableItemPrefab.uiItem;
    //     Deactivate();
    // }
    private void Awake() {
        image = transform.Find("Icon").GetComponent<Image>();
        currentAmountText = gameObject.AddComponent<Text>();
        base.Awake();
    } 

    public void SetAmount(int amount)
    {
        currentAmountText.text = amount.ToString();
    }




}