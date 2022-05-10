using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    public int maxHealth = 500;
    private int currentHealth;
    public event Action OnDie = delegate { };
    
    public event Action<int> OnDamageTake = delegate { };
    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        currentHealth = maxHealth;
    }
    

    // Update is called once per frame

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnDamageTake(damage);
        if(currentHealth <= 0f)
        {
            Reset();
            OnDie();
        }

    }
}
