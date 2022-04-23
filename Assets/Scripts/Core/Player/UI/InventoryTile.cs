using UnityEngine;
using UnityEngine.UI;

public abstract class InventoryTile : MonoBehaviour {
    public RectTransform reactibleArea;
    public Image Border;
    public Image OutlineBorder;

    public Image bg;

    protected Image image;

    public virtual void Awake() {
        Deactivate();
    }

    public virtual RectTransform GetRectTransform()
    {
        return reactibleArea;
    }

    public virtual void Deactivate(){
        Border.gameObject.SetActive(false);
        OutlineBorder.gameObject.SetActive(false);
        image.gameObject.SetActive(false);
        bg.gameObject.SetActive(false);
    }

    public void Activate(){
        Border.gameObject.SetActive(true);
        image.gameObject.SetActive(true);
        bg.gameObject.SetActive(true);
    }

    public void TurnOffBorder()
    {OutlineBorder.gameObject.SetActive(false);}
    public void TurnOnBorder()
    {OutlineBorder.gameObject.SetActive(true);}
}