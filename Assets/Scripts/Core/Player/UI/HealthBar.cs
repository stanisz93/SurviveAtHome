using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image plus;
    public StressReceiver stressReceiver;
    public float pulseSpeed =0.5f;
    private float alarmHealth = 0.2f;
    private IEnumerator alreadyRun;



    IEnumerator RunPulsingHealth()
    {
        
        float t = 0.0f;
        float _min = 0f;
        float _max = 1f;
        int direction = 1;
        while(slider.normalizedValue < 0.2)
        {

            t = t + direction * pulseSpeed * Time.deltaTime;
            plus.color = gradient.Evaluate(Mathf.Lerp(0, 1, t));
            if (t > 1.0f || t < 0f)
                {
                    float temp = _max;
                    _max = _min;
                    _min = temp;
                    direction = -direction;
                }
            yield return null;
        }
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        gradient.Evaluate(1f);
    }
    // Start is called before the first frame update
    public void ReduceValue(int damage)
    {
        slider.value = slider.value - damage;
        stressReceiver.InduceStress(1f);
        if (slider.normalizedValue < alarmHealth && alreadyRun == null)
        {
            alreadyRun = RunPulsingHealth();
            StartCoroutine(alreadyRun);
        }
    }
}