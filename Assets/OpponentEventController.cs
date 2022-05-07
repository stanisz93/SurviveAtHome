using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentEventController : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField]
    private bool blockedTask;
    void Start()
    {
        blockedTask = false;
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

    public bool TaskIsBlocked() {return blockedTask;}

}
