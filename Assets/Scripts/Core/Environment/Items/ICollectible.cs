using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public interface ICollectible
{
    void Collect();
    int GetAmount();
    ResourceType GetResourceType();

    void OnDestroy();

    Action <ICollectible> OnPickup {get; set;}
    Transform transform {get;}
    GameObject gameObject {get;}

}
