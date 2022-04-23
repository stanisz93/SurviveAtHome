using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemPickupManager: BestCandidateManager
{

    public CollectiblePopup collectiblePopup;
    private void Start() {
    }
    public void PickICollectible()
    {
        Transform best = GetBestOption();
        if(best != null)
        {
            ICollectible collectible = best.GetComponent<ICollectible>();
            if (collectible == null)
                Debug.LogError("This is not collectible!");
            RemovePotentialObject(best);
            collectiblePopup.PopUp(collectible);
            collectible.Collect();
        }

    }



}
