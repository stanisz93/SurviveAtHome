using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemPickupManager: BestCandidateManager
{

    public CollectiblePopup collectiblePopup;
    public WeaponPlaceholder weaponPlaceholder;
    private void Start() {
    }
    public bool PickItem()
    {
        Transform best = GetBestOption();
        if(best != null)
        {
            ICollectible collectible = best.GetComponent<ICollectible>();
            IDefendable defendable = best.GetComponent<IDefendable>();
            if (collectible == null && defendable == null)
            {
                Debug.LogError("This is not collectible!");
                return false;
            }
                
            else
            {
                RemovePotentialObject(best);
                if(collectible != null)
                {
                    collectiblePopup.PopUp(collectible);
                    collectible.Collect();
                }
                else if(defendable != null)
                {
                    weaponPlaceholder.SetDefendable(defendable);

                    //Here I should change player move mode to hold spear
                }
                return true;
            }
        }
        else
            return false;

    }



}
