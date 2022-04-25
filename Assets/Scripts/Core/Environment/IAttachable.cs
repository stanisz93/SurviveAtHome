using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttachable
{
    // Start is called before the first frame update

    void Unmount(); //mechanism to unmount trap/ we might want to highlight 
    //unmount option when being close to the wall

    void PlantItem(); //method to put specific object

}
