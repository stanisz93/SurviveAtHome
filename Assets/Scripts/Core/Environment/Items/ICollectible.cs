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

    void DestroyItem();

    Sprite GetImage();

    Action <ICollectible> OnPickup {get; set;}
    Transform transform {get;}
    GameObject gameObject {get;}

}
