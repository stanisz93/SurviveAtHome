using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public interface IDefendable
{

    void AttachToPlayer();

    Sprite GetImage(); //this method will demand to have image

    Vector3 GetPickPosition();
    
    Vector3 GetPickRotation();

    void ReduceEndurance();

    void AddActionOnHit(Action a);

    int GetMaxEndurance();

    int GetCurrentEndurance();


}