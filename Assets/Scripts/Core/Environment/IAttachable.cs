using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttachable
{
    // Start is called before the first frame update

    Transform GetAttachedPoint(); //give object that direct to place of set up object
    void AttachPlane(); //required to attach line}
    // Update is called once per frame
    void SwitchAttachedPlane(bool on); // method to turning on and off display of being attached

    void PlantItem(); //method to put specific object
    
    bool Forbidden();

    void Restart(); //Remove any settings, etc, cleare states to fresh start


}
