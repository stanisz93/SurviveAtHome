using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnduranceBar : MonoBehaviour
{
    // Start is called before the first frame update
    public float UpdateEnduranceTime = 0.1f;
    public Gradient gradient;
    public Slider slider;
    public Image enduranceIcon;
    public Image currentFill;
    public Image plannedFill;

    private int maxValue;

    private Coroutine currentCoroutine ;


    private void Start() {
        plannedFill.enabled = false;
    }

    public void SetInitalValue(int maxValue)
    {
        this.maxValue = maxValue;
        slider.maxValue = 1f;
        slider.value = 1f;
    }


    IEnumerator TurnOnUpdateEnduranceBar()
    {
  
        plannedFill.enabled = true;
        plannedFill.fillAmount = slider.value;
        yield return new WaitForSeconds(UpdateEnduranceTime);
        plannedFill.enabled = false;
    }

    public void SetEndurance(int value, bool isReduced)
    {
        ///regarding isReduced flag
        ///Probably I want to flip colors so that when increase endurance the new
        ///part is blue and when decrease there is yellow part a goal value
        if(isReduced)
            {
                if(currentCoroutine != null)
                    StopCoroutine(currentCoroutine);
                    currentCoroutine = StartCoroutine(TurnOnUpdateEnduranceBar());
            }
         slider.value = (float)value/maxValue;
    }


}
