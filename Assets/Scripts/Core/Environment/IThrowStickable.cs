using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class IThrowStickable : MonoBehaviour
{
    public bool damagable = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other) {
            ThrowableKnife throwItem = other.gameObject.GetComponent<ThrowableKnife>();
                
            // if(throwItem != null)
            // {
            //     SpecialKills opponent = GetComponentInParent<SpecialKills>();
            //     if(opponent != null)
            //     {
            //         throwItem.transform.parent = transform;
            //         opponent.GotKilledByThrow(throwItem.transform);
            //     }

            // }
    }
}
