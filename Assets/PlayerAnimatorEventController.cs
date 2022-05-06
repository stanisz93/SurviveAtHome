using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEventController : MonoBehaviour
{

    public PushTrigger defaultPushTrigger;
    private PushTrigger pushTrigger;
    // Start is called before the first frame update
    
    void Start()
    {
        pushTrigger = defaultPushTrigger;
    }

        
    public void TurnOnPushCollider()
    {
        pushTrigger.collider.enabled = true;
    }

    public void TurnOffPushCollider()
    {
        pushTrigger.collider.enabled = false;
    }
    public void SetPushCollider(PushTrigger trigger)
    {
            pushTrigger = trigger;
    }

    public void SetToDefaultPushTrigger()
    {
        pushTrigger = defaultPushTrigger;
    }


    // Update is called once per frame
}
