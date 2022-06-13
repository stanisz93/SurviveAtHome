using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeThrow : MonoBehaviour, IThrowable
{
    // Start is called before the first frame update
    
    public TriggerType triggerType {get 
            {return TriggerType.Melee;}}
    public float blockMovement{ get {return 0.4f;}}

    public float releaseTriggerTime{ get {return 0.4f;}}

    public string animName{ get {return "Throw";}}

    public float distanceLeft{ get {return Mathf.Infinity;}}




    // Update is called once per frame
    public void ReleaseAttack()
    {
    
    }
}
