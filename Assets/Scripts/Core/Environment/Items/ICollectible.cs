using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public interface ICollectible
{
    void Collect();
    int GetAmount();
    ResourceType GetResourceType();

    void DestroyItem(float delay=0.0f, bool usingAnimation=true);

    Sprite GetImage();

    Action <ICollectible> OnPickup {get; set;}
    Transform transform {get;}
    GameObject gameObject {get;}

}
