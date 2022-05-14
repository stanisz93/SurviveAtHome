using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public interface IDefendable
{

    void AttachToPlayer();

    Sprite GetImage(); //this method will demand to have image


    void ReduceEndurance();

    void AddActionOnHit(Action a);

    int GetMaxEndurance();

    int GetCurrentEndurance();


}