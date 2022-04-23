using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttachable
{
    // Start is called before the first frame update


    void AttachPlane(); //required to attach line}
    // Update is called once per frame
    void SwitchAttachedPlane(bool on);

}
