using System.Collections;
using System.Collections.Generic;


public class Indicator
{
    private float maxValue;
    private float currentValue;
    private float increaseStep;
    private float decreaseStep;
    private float initialValue;

    private bool initiallyActive;
    private bool active;

    private bool reachMax=false;
    private bool reachMin=false;
    public Indicator(float initialValue, float maxValue, float increaseStep, float decreaseStep, bool initiallyActive)
    {
        this.maxValue = maxValue;
        this.initialValue = initialValue;
        this.currentValue = initialValue;
        this.increaseStep = increaseStep;
        this.decreaseStep = decreaseStep;
        this.initiallyActive = initiallyActive;
        this.active = initiallyActive;
    }

    public void Reset() 
    {
        ResetValue();
        active = initiallyActive;
    }
    public void ResetValue()
    {
        currentValue = initialValue;
    }
    public float GetCurrentValue() {return currentValue;}
    public bool ReachedMax() { return reachMax;}

    public void Deactivate() { active = false; }
    public void Activate() {
        active = true;
        }
    public bool ReachedMin() { return reachMin;}
    public void SetToZero() {currentValue = 0f;}
    public void SetToMax() {currentValue = maxValue;}

    public bool isActive() {return active;}
    public float GetMaxValue() {return maxValue;}
    public void Increase()
    {
        currentValue += increaseStep;
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
        currentValue -= decreaseStep;
        if(currentValue <= 0f)
        {
            currentValue = 0f;
            if(!reachMin) reachMin = true;
        }
        else if(reachMin) reachMin = false;
    }


}