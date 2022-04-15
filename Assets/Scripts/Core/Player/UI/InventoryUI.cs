using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour
{

    public Camera mainCam;
    public Image backpack;

    private bool isOpen = false;
    private List<MainBlockTile> tiles;

    private MainBlockTile currentBlockTile = null;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new List<MainBlockTile>(gameObject.GetComponentsInChildren<MainBlockTile>());
        LeftInventory();
        // GoToInventory();
    }

    // Update is called once per frame
    public void ContainsAnyTile(Vector3 mousePos)
    {
        foreach(MainBlockTile tile in tiles)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(tile.GetRectTransform(), mousePos))
                SwampFocus(tile);
        }
        if(currentBlockTile != null)
            currentBlockTile.ContainsAnySubtile(mousePos);
    

    }

    public void ChooseSubtile(Vector3 mousePos)
    {
        /// Here mechanique for manage all subtiles
        
    }



    void SwampFocus(MainBlockTile newTile)
    {
        if(currentBlockTile != null)
            currentBlockTile.SwitchFocus(false);
        currentBlockTile = newTile;
        currentBlockTile.SwitchFocus(true);
    }

    void ResetUI()
    {
        if(currentBlockTile != null)
        {
            currentBlockTile.TurnOffBorder();
            currentBlockTile.DeactivateTileGroup();
        }
        currentBlockTile = null;
    }

    public void GoToInventory()
    {
        if(!isOpen)
        {
            backpack.enabled = true;
            foreach(MainBlockTile tile in tiles)
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
        foreach(MainBlockTile tile in tiles)
        {
            tile.Deactivate();
        }
        isOpen = false;
    }
}
