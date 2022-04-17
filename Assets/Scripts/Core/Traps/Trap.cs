using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Trap : MonoBehaviour {

    public Image uiImage;
    [SerializeField]
    private Dictionary<string, int> requireResources
    {
        get {return requireResources; }
        set {requireResources = value; }
    }


}

