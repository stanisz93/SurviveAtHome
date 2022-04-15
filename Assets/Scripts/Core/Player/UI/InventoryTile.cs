using UnityEngine;
using UnityEngine.UI;

public abstract class InventoryTile : MonoBehaviour {
    public RectTransform reactibleArea;
    public Image Border;
    public Image OutlineBorder;

    protected Image image;

    protected virtual void Awake() {
        Deactivate();
    }

    public virtual RectTransform GetRectTransform()
    {
        return reactibleArea;
    }

    public virtual void Deactivate(){
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