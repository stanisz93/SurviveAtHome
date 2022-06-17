using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Endurance : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int maxEndurance = 500;

    [SerializeField]
    float updateEnduranceFrequency = 0.05f;

    [SerializeField]
    int RestoreValueAtEachStep = 10; ///This is the max speed of restoring value
    [SerializeField]
    float restoreHealthFrequency = 0.4f;

    public EnduranceBar enduranceBar;

    private  Queue<int> enduranceChangeEvents;
    private int maxQueueLimit = 20;
    private int _currentEndurance; //Do not touch it except Update loop
    
    void Start()
    {
        Reset();
        enduranceChangeEvents = new Queue<int>();
        enduranceBar.SetInitalValue(maxEndurance);
        StartCoroutine(RestoreEndurance());
        StartCoroutine(UpdateEndurance());
    }

    public void Reset()
    {
        _currentEndurance = maxEndurance;
    }

    public void ReduceEndurance(int amount)
    {
        if(enduranceChangeEvents.Count < maxQueueLimit)
            enduranceChangeEvents.Enqueue(-amount);
    }

    public void IncreaseEndurance(int amount)
    {
        if(enduranceChangeEvents.Count < maxQueueLimit)
            enduranceChangeEvents.Enqueue(amount);
    }

    

    public int GetCurrentEndurance() => _currentEndurance;

    IEnumerator RestoreEndurance()
    {
        while(true)
        {
            yield return new WaitForSeconds(restoreHealthFrequency);
            if(_currentEndurance < maxEndurance)
                 enduranceChangeEvents.Enqueue(RestoreValueAtEachStep);
        }
    }

    IEnumerator UpdateEndurance(){
        while(true)
        {
            yield return new WaitForSeconds(updateEnduranceFrequency);
            if(enduranceChangeEvents.Count != 0)
            {
                int val = enduranceChangeEvents.Dequeue();
                _currentEndurance = (int)Mathf.Clamp(_currentEndurance + val, 0f, maxEndurance);
                enduranceBar.SetEndurance(_currentEndurance, val < 0);
            }
        }
    }
    

    // Update is called once per frame

}
