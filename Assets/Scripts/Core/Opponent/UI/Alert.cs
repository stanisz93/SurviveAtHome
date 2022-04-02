using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;

    public void SetDefault(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = 0;
    }
    // Start is called before the first frame update
    public void SetAlertLevel(float alertLevel)
    {
        slider.value = alertLevel;
    }
}
