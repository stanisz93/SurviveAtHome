using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AttachableWall : MonoBehaviour, IAttachable
{


public void Unmount()
{
    Debug.Log("Unmounting");
} //mechanism to unmount trap/ we might want to highlight 
//unmount option when being close to the wall

public void PlantItem()
{
     Debug.Log("Planting");
}







}
