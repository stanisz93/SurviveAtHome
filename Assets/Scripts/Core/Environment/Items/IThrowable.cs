using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IThrowable
{
    // Start is called before the first frame update
    
    string distantAnimName {get;}
    

    //This method is all operation 
    //on player side
    // void ReleaseThrow();

    void SetThrowTarget(Vector3 target);

    //this operation reffers to object
    //that is throwed// physics
    //detached from player etc
    void Throw();
}
