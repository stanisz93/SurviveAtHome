using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldWeapon : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private byte alpha = 200;
    public Image weaponImage;

    void Start()
    {
        
    }

    // Update is called once per frame
    public void SetWeaponImage(Sprite img)
    {
        weaponImage.sprite = img;
        weaponImage.color = new Color32(255, 255, 225, alpha);
        DoTweenUtils.PoopUpImage(weaponImage, 1f);
    }


}
