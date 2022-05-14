using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public interface IDefendable
{

    void Collect();

    Sprite GetImage(); //this method will demand to have image


    void ReduceEndurance();


    int GetMaxEndurance();

    int GetCurrentEndurance();

    void Drop();


}