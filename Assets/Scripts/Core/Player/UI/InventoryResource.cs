using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryResource : MonoBehaviour
{
    // Start is called before the first frame update    
    public Image img;
    public Text txt;

    public ResourceType resourceType;

    // Update is called once per frame
    void Awake()
    {
        txt.text = "0";
    }
    public void Switch(bool on)
    {
        img.enabled = on;
        txt.enabled = on;
    }

    public void SetValue(int value)
    {
        txt.text = value.ToString();
    }
    
}
