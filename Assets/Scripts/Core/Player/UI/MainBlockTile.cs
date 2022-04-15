using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainBlockTile : InventoryTile
{

    private List<ItemTile> subtiles;
    private ItemTile currentSubtile = null;
    private void Awake() {
        image = transform.Find("Icon").GetComponent<Image>();
        subtiles = new List<ItemTile>(gameObject.GetComponentsInChildren<ItemTile>());

        base.Awake();
    }

    // Update is called once per frame


    public void ContainsAnySubtile(Vector3 mousePos)
    {
        foreach(ItemTile subtile in subtiles)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(subtile.GetRectTransform(), mousePos))
                SwampSubtileFocus(subtile);
        } 
    }

    public void SwampSubtileFocus(ItemTile newSubtile)
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

    public void DeactivateTileGroup()
    {
        if(subtiles != null)
        {
            if(subtiles.Count > 0)
            {
                foreach(ItemTile subtile in subtiles)
                {
                    subtile.Deactivate();
                }
            }
        }
    }

    public void ActivateTileGroup()
    {
        if(subtiles.Count > 0)
        {
            currentSubtile = subtiles[0];
            foreach(ItemTile subtile in subtiles)
            {
                subtile.Activate();
            }
        }
    }
}
