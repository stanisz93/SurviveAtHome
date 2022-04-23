using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(ICraftable))]
public class ItemTile : InventoryTile 
{
    public float CraftSpeed = 0.1f;
    public Text currentAmount;
    public Text requirement;
    public Image actionSquare;
    private Slider craftSlider;
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
        craftSlider = GetComponent<Slider>();
        craftSlider.value = 0f;
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

    public void ResetCraftProgress()
    {
        craftSlider.value = 0f;
    }
    public IEnumerator CraftAnimate()
    {
        float t = 0.0f;
        float _min = 0f;
        float _max = 1f;
        
        while(craftSlider.normalizedValue < 1)
        {
            t = t + CraftSpeed * Time.deltaTime;
            craftSlider.value = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        ResetCraftProgress();
    }

    public IEnumerator TryToCraft() //here method to craft specific item
    {
        bool success;
        Debug.Log("Trying crafting");
        
        if(inventory.SatisfyRequirement(craftable.GetRequirements()))
            {
            //here crafting whree coroutine
                yield return CraftAnimate();
                inventory.AddItem(this.gameObject);
                SetAmount(inventory.crafts[craftable.GetCraftType().ToString()]);
                DoTweenUtils.PoopUpImage(image);
                DoTweenUtils.PoopUpTextTween(currentAmount, Color.green);
                Debug.Log("Done crafting!");
            }
        else
            {
                Debug.Log("No resources!");
                DoTweenUtils.PoopUpTextTween(requirement, Color.red);
                yield break;

                //HERE maybe red animation!
            }
            
    }









}