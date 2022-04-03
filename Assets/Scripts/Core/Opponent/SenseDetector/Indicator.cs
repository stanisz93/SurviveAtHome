using System.Collections;
using System.Collections.Generic;


public class Indicator
{
    private float maxValue;
    private float currentValue;
    private float step;
    private float initialValue;

    private bool reachMax=false;
    private bool reachMin=false;
    public Indicator(float initialValue, float maxValue, float step)
    {
        this.maxValue = maxValue;
        this.initialValue = initialValue;
        this.currentValue = initialValue;
        this.step = step;
    }

    public void ResetValue() {currentValue = initialValue;}
    public float GetCurrentValue() {return currentValue;}
    public bool ReachedMax() { return reachMax;}

    public bool ReachedMin() { return reachMin;}
    public void SetToZero() {currentValue = 0f;}

    public void Increase()
    {
        currentValue += step;
        if(reachMin) reachMin = false;
        if(currentValue >= maxValue)
        {
            currentValue = maxValue;
            if(!reachMax) reachMax = true;
        }
        else if (reachMax) reachMax = false;
    }

    public void Decrease()
    {
         if(reachMax) reachMax = false;
        currentValue -= step;
        if(currentValue <= 0f)
        {
            currentValue = 0f;
            if(!reachMin) reachMin = true;
        }
        else if(reachMin) reachMin = false;
    }


}