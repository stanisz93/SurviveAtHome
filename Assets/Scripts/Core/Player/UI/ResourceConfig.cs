using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class ResourceConfig : MonoBehaviour
{
    // Start is called before the first frame update
    private TextMeshPro textMesh;
    private GameObject objRef;
    
    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
        textMesh.enabled = false;
    }
    public void Setup(int amount, string resType)
    {
        if(!textMesh.enabled)
            textMesh.enabled = true;
        textMesh.SetText($"+ {amount} {resType}");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(textMesh.DOFade(0f, 1.2f)).Join(transform.DOMoveY(3, 2f, false)).AppendCallback(() => Destroy(gameObject));
    }

    // Update is called once per frame

}
