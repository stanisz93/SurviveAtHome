using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryTile : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform reactibleArea;
    public Image Border;
    public Image OutlineBorder;
    public Image image;


    // Update is called once per frame

    public RectTransform GetRectTransform()
    {
        return reactibleArea;
    }

    public void Deactivate(){
        Border.enabled = false;
        OutlineBorder.enabled = false;
        image.enabled = false;
    }

    
    public void Activate(){
        Border.enabled = true;
        image.enabled = true;
    }
   public void TurnOffBorder()
    {OutlineBorder.enabled = false;}
    public void TurnOnBorder()
    {OutlineBorder.enabled = true;}

}
