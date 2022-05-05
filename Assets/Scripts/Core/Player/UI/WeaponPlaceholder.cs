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
    private IDefendable defendable;

    void Start()
    {
        endurancePointer.enabled = false;
    }

    // Update is called once per frame
    public void SetWeaponImage()
    {
        weaponImage.sprite = defendable.GetImage();
        DoTweenUtils.PoopUpImage(weaponImage, 0.8f);
    }

    public void RemoveWeapon()
    {
        this.defendable = null;
        weaponImage.sprite = null;
        weaponImage.color = new Color32(255, 255, 255, 0);
        DoTweenUtils.PoopUpImage(weaponImage, 0.8f);
    }

    public void SetDefendable(IDefendable defendable)
    {
        this.defendable = defendable;
        this.defendable.AttachToPlayer();
        this.defendable.AddActionOnHit(UpdateEndurance);
        SetWeaponImage();
        SetMaxEndurance();
    }

    public void SetMaxEndurance()
    {

        slider.maxValue = defendable.GetMaxEndurance();
        slider.value = defendable.GetMaxEndurance();
        endurancePointer.enabled = true;
        endurancePointer.color = gradient.Evaluate(0f);

    }

    public void UpdateEndurance()
    {
        var currEndurance = (float)defendable.GetCurrentEndurance();
        slider.value = currEndurance;
        DoTweenUtils.PoopUpImage(endurancePointer, 0.16f, 0.16f);
        var maxVal = (float)defendable.GetMaxEndurance();
        var ratio = (maxVal - currEndurance) / maxVal;
        endurancePointer.color = gradient.Evaluate(ratio);
        if( slider.value <= 0)
            RemoveWeapon();
    }


}
