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

    public List<InventoryTile> subtiles;
    private InventoryTile currentSubtile = null;

    private void Start() {
        Deactivate();
    }

    // Update is called once per frame

    public RectTransform GetRectTransform()
    {
        return reactibleArea;
    }

    public void SwampSubtileFocus(InventoryTile newSubtile)
    {
        if(subtiles.Contains(newSubtile))
        {
            if(currentSubtile != null)
            {
                currentSubtile.TurnOffBorder();
            }
            newSubtile.TurnOnBorder();
            currentSubtile = newSubtile;
        }
        else
            throw new System.Exception($"Subtile list doen's contain {newSubtile}");
    }

    public void Deactivate(){
        Border.enabled = false;
        OutlineBorder.enabled = false;
        image.enabled = false;
    }


    public void SwitchFocus(bool on)
    {
        if(on)
        {
            TurnOnBorder();
            ActivateTileGroup();
        }
        else
        {
            TurnOffBorder();
            DeactivateTileGroup();
        }

    }

    
    public void Activate(){
        Border.enabled = true;
        image.enabled = true;
    }
    public void TurnOffBorder()
    {OutlineBorder.enabled = false;}
    public void TurnOnBorder()
    {OutlineBorder.enabled = true;}

    public void DeactivateTileGroup()
    {
        if(subtiles.Count > 0)
        {
            foreach(InventoryTile subtile in subtiles)
            {
                subtile.Deactivate();
            }
        }
    }

    public void ActivateTileGroup()
    {
        if(subtiles.Count > 0)
        {
            currentSubtile = subtiles[0];
            foreach(InventoryTile subtile in subtiles)
            {
                subtile.Activate();
            }
        }
    }
}
