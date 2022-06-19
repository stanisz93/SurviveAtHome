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
    [SerializeField]
    float timeWithoutRunForRegeneration = 3f;

    public EnduranceBar enduranceBar;

    private  Queue<int> enduranceChangeEvents;
    private int maxQueueLimit = 20;
    private int _currentEndurance; //Do not touch it except Update loop
    private Character character;

    void Start()
    {
        Reset();
        character = GetComponent<Character>();
        enduranceChangeEvents = new Queue<int>();
        enduranceBar.SetInitalValue(maxEndurance);
        StartCoroutine(RestoreEndurance());
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

    IEnumerator TimeWithoutRunning()
    {
        float timer = 0f;
        yield return new WaitUntil(() => character.GetMovement() != MovementMode.Running);
        while(timer <= timeWithoutRunForRegeneration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
    }

    IEnumerator RestoreEndurance()
    {
        while(true)
        {
            if(_currentEndurance < maxEndurance)
            {
                if (character.GetMovement() == MovementMode.Running)
                    yield return TimeWithoutRunning();
                else
                {
                    IncreaseEndurance(RestoreValueAtEachStep);
                    yield return new WaitForSeconds(restoreHealthFrequency);
                }
            }
            else
                yield return null;
        }
    }

    private void Update(){

            if(enduranceChangeEvents.Count != 0)
            {
                int val = enduranceChangeEvents.Dequeue();

                int newEndurance = (int)Mathf.Clamp(_currentEndurance + val, 0f, maxEndurance);
                if(newEndurance != _currentEndurance)
                    {
                        _currentEndurance = newEndurance;
                        enduranceBar.SetEndurance(_currentEndurance, val < 0);
                    }
            }
    }
    

    // Update is called once per frame

}
