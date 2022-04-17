using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(Collider))]
public class SpoonItem : ResourceItem
{
    public int amountSet;
    public override int amount 
    {
        get {return amountSet;}
        set {}
    }
    private void Awake() 
    {
        base.Awake();
        enumName = ResourceType.Metal;
    }
}
