using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetDefault(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = 0;
    }
    // Start is called before the first frame update
    public void SetAlertLevel(float alertLevel, bool founded)
    {
        if (founded)
        fill.color = gradient.Evaluate(1f);
        else
        fill.color = gradient.Evaluate(0f);
        
        slider.value = alertLevel;
    }
}
