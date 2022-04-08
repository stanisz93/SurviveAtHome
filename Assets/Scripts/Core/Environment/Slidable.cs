using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slidable : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform slidePoint;
    public Transform landPoint;
    public Transform closer;
    public Transform further;
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other) {
        PlayerTriggers c = other.GetComponent<PlayerTriggers>();
        if (c != null)
        {
            Vector3 plrPos = c.transform.position;
            closer = slidePoint;
            further = landPoint;
            if(Vector3.Distance(landPoint.position, plrPos) < Vector3.Distance(slidePoint.position, plrPos))
            {
                closer = landPoint;
                further = slidePoint;
            }
            c.Slidable = this;
            // tODO: If direction is wrong the sliding behave not physical
            // var dot = Vector3.Dot(transform.forward, (c.transform.position - transform.position).normalized);
            // Debug.Log($"Dot prodict: {dot}");
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
 

    public Vector3 GetSlidePoint()
    {
        return closer.position;
    }

    public Vector3 GetEndSlidePoint()
    {
        return further.position;
    }
}
