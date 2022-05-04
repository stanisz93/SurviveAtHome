using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDefendable
{
    Action <IDefendable> OnPickup {get; set;}

    void AttachToPlayer();

    Vector3 GetPickPosition();
    
    Vector3 GetPickRotation();
}