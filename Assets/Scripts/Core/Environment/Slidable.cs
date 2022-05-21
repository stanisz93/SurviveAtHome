using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Slidable : MonoBehaviour
{
    // Start is called before the first frame update
    public  float MoveToPointTime = 0.1f;
    public  float MoveToEndPointTime = 0.8f;
    public  float ReleaseTime = 1f;

    public  float slideThreshold = 0.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other) {
        PlayerTriggers c = other.GetComponent<PlayerTriggers>();
        if (c != null)
        {
            c.Slidable = this;
        }
    }
    void OnTriggerExit(Collider other)
    {
        PlayerTriggers c = other.GetComponent<PlayerTriggers>();
        if (c != null)
        {
            c.Slidable = null;
        }
    }

}
