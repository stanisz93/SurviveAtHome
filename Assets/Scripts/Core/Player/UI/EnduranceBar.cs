using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class EnduranceBar : MonoBehaviour
{
    // Start is called before the first frame update
    public float UpdateEnduranceTime = 0.1f;

    public float criticalFraction = 0.05f;
    public Gradient boostGradient;
    public Slider slider;
    public Image enduranceIcon;
    public Image currentFill;
    public Image plannedFill;

    public Image AlertEffect;

    private int maxValue;

    private Coroutine currentCoroutine;
    private Tween alertTween;

    private float A, B, C;

    private void Start() {
        plannedFill.enabled = false;
        AlertEffect.enabled = false;
        alertTween = AlertEffect.DOFade(0f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        alertTween.Pause();       
        }

    void CalculateCoeffs(float valueAtZero, float ValueAtHalf, float valueAtOne)
    {
        //taken from https://stackoverflow.com/questions/7246622/how-to-create-a-slider-with-a-non-linear-scale
            A = (valueAtZero * valueAtOne - Mathf.Pow(ValueAtHalf, 2)) / (valueAtZero - 2 * ValueAtHalf + valueAtOne);
            B = Mathf.Pow(ValueAtHalf - valueAtZero, 2) / (valueAtZero - 2 * ValueAtHalf + valueAtOne);
            C = 2 * Mathf.Log((valueAtOne - ValueAtHalf) / (ValueAtHalf - valueAtZero));
    }

    public void SetInitalValue(int maxValue)
    {
        this.maxValue = maxValue;
        CalculateCoeffs(0.0f, (float)maxValue / 5, (float)maxValue);
        slider.maxValue = 1f;
        slider.value = 1f;
    }


    IEnumerator TurnOnUpdateEnduranceBar(bool isReduced)
    {
        plannedFill.color = boostGradient.Evaluate(Convert.ToInt32(isReduced));
        plannedFill.enabled = true;
        plannedFill.fillAmount = slider.value;
        yield return new WaitForSeconds(UpdateEnduranceTime);
        plannedFill.enabled = false;
    }

    void ToogleAlertTween(bool turnOn)
    {
        if(turnOn)
            {
                alertTween.Play();
            }
        else
            alertTween.Pause();

        AlertEffect.enabled = turnOn;


    }



    public void SetEndurance(int value, bool isReduced)
    {
        ///regarding isReduced flag
        ///Probably I want to flip colors so that when increase endurance the new
        ///part is blue and when decrease there is yellow part a goal value
        float fraction = (float)value/maxValue;
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(TurnOnUpdateEnduranceBar(isReduced));
        ToogleAlertTween(fraction < criticalFraction);
        // slider.value = (float)value/maxValue;

        ///when value chagne use this
        //https://mycurvefit.com/?fbclid=IwAR0sBSbdKzSa6RRBErHNBHB7MCSf08YsuIe0zy350bLajWzzVRnjo3a3mos
        //c/(e^(-x))
        slider.value = Mathf.Log((value - A) / B) / C;
    }


}
