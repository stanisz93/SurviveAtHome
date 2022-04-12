using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour
{

    public List<InventoryTile> tiles;
    public Camera mainCam;
    public Image backpack;

    private bool isOpen = false;

    private InventoryTile currentTile = null;

    // Start is called before the first frame update
    void Start()
    {
        LeftInventory();
        // GoToInventory();
    }

    // Update is called once per frame
    public void ContainsAnyTile(Vector3 mousePos)
    {
        foreach(InventoryTile tile in tiles)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(tile.GetRectTransform(), mousePos))
                SwampFocus(tile);
        }
        if(currentTile != null)
            ContainsAnySubtile(mousePos);

    }

    public void ChooseSubtile(Vector3 mousePos)
    {
        /// Here mechanique for manage all subtiles
        
    }

    
    void ContainsAnySubtile(Vector3 mousePos)
    {
        foreach(InventoryTile subtile in currentTile.subtiles)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(subtile.GetRectTransform(), mousePos))
                currentTile.SwampSubtileFocus(subtile);
        } 
    }


    void SwampFocus(InventoryTile newTile)
    {
        if(currentTile != null)
            currentTile.SwitchFocus(false);
        currentTile = newTile;
        currentTile.SwitchFocus(true);
    }

    void ResetUI()
    {
        if(currentTile != null)
        {
            currentTile.TurnOffBorder();
            currentTile.DeactivateTileGroup();
        }
        currentTile = null;
    }

    public void GoToInventory()
    {
        if(!isOpen)
        {
            backpack.enabled = true;
            foreach(InventoryTile tile in tiles)
            {
                tile.Activate();
            }
            isOpen = true;
        }
    }

    public void LeftInventory()
    {
        backpack.enabled = false;

        ResetUI();
        foreach(InventoryTile tile in tiles)
        {
            tile.Deactivate();
        }
        isOpen = false;
    }
}
