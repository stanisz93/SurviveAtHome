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
    private List<InventoryResource> resources;
    private Dictionary<string, InventoryResource> inventoryResourceDict;


    private MainBlockTile currentBlockTile = null;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new List<MainBlockTile>(gameObject.GetComponentsInChildren<MainBlockTile>());
        resources = new List<InventoryResource>(gameObject.GetComponentsInChildren<InventoryResource>());
        inventoryResourceDict = new Dictionary<string, InventoryResource>();
        foreach(var res in resources)
        {
            inventoryResourceDict.Add(res.resourceType.ToString(), res);
        }
        
        LeftInventory();
        // GoToInventory();
    }

    // Update is called once per frame
    public InventoryTile ContainsAnyTile(Vector3 mousePos)
    {
        InventoryTile candidateTile = null;
        foreach(MainBlockTile tile in tiles)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(tile.GetRectTransform(), mousePos))
                {
                    SwampFocus(tile);
                    candidateTile = tile;
                }
        }
        if(currentBlockTile != null)
        {
            InventoryTile tempTile = currentBlockTile.ContainsAnySubtile(mousePos);
            if (tempTile != null)
                candidateTile = tempTile;
        }
        return candidateTile;

    }

    public void ChooseSubtile(Vector3 mousePos)
    {
        /// Here mechanique for manage all subtiles
        
    }

    public void UpdateMetalResource(int value)
    {
        inventoryResourceDict[ResourceType.Metal.ToString()].SetValue(value);
    }

    
    public void UpdateClothResource(int value)
    {
        inventoryResourceDict[ResourceType.Cloth.ToString()].SetValue(value);
    }

    public void UpdateResourcesUI(ResourceType resType, int value)
    {
        if (resType == ResourceType.Metal)
        {
            UpdateMetalResource(value);
        }
        else if(resType == ResourceType.Cloth)
        {
            UpdateClothResource(value);
        }
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
            foreach(InventoryResource res in resources)
        {
            res.Switch(true);
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
        foreach(InventoryResource res in resources)
        {
            res.Switch(false);
        }
        isOpen = false;
    }
}
