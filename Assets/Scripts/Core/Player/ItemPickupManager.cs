using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager: BestCandidateManager
{

    public bool PickItem()
    {
        Transform best = GetBestOption();
        if(best != null)
        {
            ICollectible collectible = best.GetComponent<ICollectible>();
            if (collectible == null)
            {
                Debug.LogError("This is not collectible!");
            }
                
            else
            {
                RemovePotentialObject(best);
                if(collectible != null)
                {
                    collectible.Collect();
                    return true;
                    
                }

            }
        }
        return false;

    }



}
