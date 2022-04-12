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
                            ChangeTile(tile);
        } 

    }


    void ChangeTile(InventoryTile newTile)
    {
        if(currentTile != null)
            currentTile.TurnOffBorder();
        currentTile = newTile;
        currentTile.TurnOnBorder();
    }

    void ResetUI()
    {
        if(currentTile != null)
            currentTile.TurnOffBorder();
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
