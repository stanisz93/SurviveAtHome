using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPlaceholder : MonoBehaviour
{
    // Start is called before the first frame update
    public Gradient gradient;
    public Slider slider;
    [SerializeField]
    public Image weaponImage;
    public Image endurancePointer;
    private DefendItem defendItem;

    void Start()
    {
        endurancePointer.enabled = false;
    }

    // Update is called once per frame
    void SetWeaponImage()
    {
        weaponImage.sprite = this.defendItem.GetImage();
        weaponImage.color = new Color32(255, 255, 255, 255);
        DoTweenUtils.PoopUpImage(weaponImage, 0.8f);
    }

    public void RemoveWeapon()
    {
        this.defendItem = null;
        weaponImage.sprite = null;
        weaponImage.color = new Color32(255, 255, 255, 0);
        DoTweenUtils.PoopUpImage(weaponImage, 0.8f);
        RemoveEndurance();
    }

    public void AttachWeapon(ICollectible collectible)
    {
        if(this.defendItem != null)
           defendItem.Drop();
        defendItem = (DefendItem)collectible;
        float initialVal = (float)this.defendItem.GetInitialEndurance();
        slider.maxValue = initialVal;
        endurancePointer.enabled = true;
        SetWeaponImage();
        UpdateEndurance(defendItem.GetCurrentEndurance());
        // UpdateEndurance();
    }


    void RemoveEndurance()
    {
        slider.maxValue = 0f;
        slider.value = 0f;
        endurancePointer.enabled = false;
    }

    public void UpdateEndurance(float endurance)
    {
        slider.value = endurance;
        DoTweenUtils.PoopUpImage(endurancePointer, 0.16f, 0.16f);
        var maxVal = (float)defendItem.GetInitialEndurance();
        var ratio = (maxVal - endurance) / maxVal;
        endurancePointer.color = gradient.Evaluate(ratio);
    }


}
