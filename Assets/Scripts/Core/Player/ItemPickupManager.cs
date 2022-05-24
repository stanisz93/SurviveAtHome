using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager: BestCandidateManager
{

    public Action PickItem()
    {
        Action onCollectProcess = null;
        Transform best = GetBestOption();
        if(best != null)
        {
            ICollectible collectible = best.GetComponent<ICollectible>();
            IDefendable defendable = best.GetComponent<IDefendable>();
            if (collectible == null && defendable == null)
            {
                Debug.LogError("This is not collectible!");
            }
                
            else
            {
                RemovePotentialObject(best);
                if(collectible != null)
                {
                    onCollectProcess = collectible.Collect;
                }
                else if(defendable != null)
                {
                    onCollectProcess = defendable.Collect;
                    // weaponPlaceholder.SetDefendable(defendable);

                    //Here I should change player move mode to hold spear
                }
            }
        }
        return onCollectProcess;

    }



}
