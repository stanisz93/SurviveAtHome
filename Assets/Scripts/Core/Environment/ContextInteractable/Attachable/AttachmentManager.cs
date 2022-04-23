using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttachmentManager: BestCandidateManager
{

public IAttachable currentAttachable = null;

public void AttachToObject()
{
    Transform best = GetBestOption();
    if(best != null)
    {
        IAttachable attachable = best.GetComponent<IAttachable>();
        if (attachable == null)
            Debug.LogError("This is not collectible!");
        attachable.AttachPlane();
    }
}

public override IEnumerator CheckPotentialOptions()
{
        while(true)
        {
            SetBestOption();
            if(bestOption != null)
            {
                IAttachable attachable = bestOption.GetComponent<IAttachable>();
                if(currentAttachable != attachable)
                    {
                        if(currentAttachable != null)
                            currentAttachable.SwitchAttachedPlane(false);
                        attachable.SwitchAttachedPlane(true);
                        currentAttachable = attachable;
                    }
            }
            yield return new WaitForSeconds(0.2f);
        }
}
}