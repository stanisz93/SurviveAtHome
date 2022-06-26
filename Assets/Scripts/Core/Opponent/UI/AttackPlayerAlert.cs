using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AttackPlayerAlert : MonoBehaviour
{
    // Start is called before the first frame update

    public Image alertIcon;

    public void Start()
    {
        alertIcon.enabled = false;
        var tempColor = alertIcon.color;
        tempColor.a = 0f;
        alertIcon.color = tempColor;
    }
    // Start is called before the first frame update
    public IEnumerator TurnOnAlertBeforeAttack()
    {
        alertIcon.enabled = true;

        Tween tween = alertIcon.DOFade(1f, 0.1f).SetLoops(4, LoopType.Yoyo);
        yield return tween.WaitForCompletion();
    }
}
