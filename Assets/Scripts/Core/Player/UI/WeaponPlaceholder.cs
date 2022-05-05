using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPlaceholder : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    [SerializeField]
    private byte alpha = 200;
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
        weaponImage.color = new Color32(255, 255, 225, alpha);
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
        endurancePointer.color = new Color32(255, 255, 255, 255);

    }

    public void UpdateEndurance()
    {
        slider.value = defendable.GetCurrentEndurance();
        DoTweenUtils.PoopUpImage(endurancePointer, 0.16f, 0.16f);
        if (slider.value < 0.4 * defendable.GetMaxEndurance())
        {
            endurancePointer.color = new Color32(138, 3, 3, 255);
        }
    }


}
