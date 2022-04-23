using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableGenerator : MonoBehaviour
{

    public Rigidbody hook;

    public GameObject linkPrefab;

    // public BreakPoint breakPoint;
    public int links = 10;
    public List<Transform> points;
    private GameObject lastPoint;
    private bool broken = false;
    // Start is called before the first frame update

    // Update is called once per frame
    void CalulateExpectedLinks()
    {
        // float dist = Vector3.Distance(hookA.transform.position, hookB.transform.position);
        // links = Mathf.RoundToInt(dist / 0.3f) - 1;
        Debug.Log($"links: {links}");

    }


    public GameObject GetLastPoint()
    {
        return lastPoint;
    }

    public void RemovePoint(Link link)
    {
        links -= 1;
        broken = true;
        points.Remove(link.transform);
    }



    public bool IsBroken()
    {
        return broken;
    }

    public void GenerateWire()
    {
        Rigidbody prevRB = hook;
        for(int i = 0; i < links; i++)
        {
            GameObject link = Instantiate(linkPrefab, transform);
            HingeJoint joint = link.GetComponent<HingeJoint>();
            joint.connectedBody = prevRB;
            points.Add(link.transform);
            // if (i < links - 1)
            // {
                prevRB = link.GetComponent<Rigidbody>();
            // }
            // else
            // {
            //     breakPoint.ConnectRopeEnd(link.GetComponent<Rigidbody>());

            // }
            if (i == links - 1)
                lastPoint = link;

        }
        
    
    }
}
