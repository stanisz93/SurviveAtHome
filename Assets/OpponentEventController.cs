using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentEventController : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField]
    private bool blockedTask;
    
    public Opponent opponent;
    private Character character;

    void Start()
    {
        blockedTask = false;
        character = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    // Update is called once per frame
    public void StartTask()
    {
        blockedTask = true;
    }
    public void Stoptask()
    {
        blockedTask = false;
    }

    public void InvokePlayerGotHit()
    {
        character.ReduceHealth(opponent.damage);
    }


    public bool TaskIsBlocked() {return blockedTask;}

}
