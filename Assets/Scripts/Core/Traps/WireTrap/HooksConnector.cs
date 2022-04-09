using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HooksConnector : MonoBehaviour
{
    // Start is called before the first frame update
    public CableGenerator hookA;
    public CableGenerator hookB;
    bool runOnce = false;

    void ConnectLastPointsOfTwoHooks()
    {
        GameObject linkA = hookA.GetLastPoint();
        float yoffset = linkA.GetComponent<Link>().GetYOffset();
        HingeJoint jointA = linkA.AddComponent<HingeJoint>();
        jointA.connectedBody = hookB.GetLastPoint().GetComponent<Rigidbody>();
        jointA.autoConfigureConnectedAnchor = false;
        jointA.anchor = Vector3.zero;
        jointA.connectedAnchor = new Vector3(0f, -yoffset, 0f);

        HingeJoint jointB = hookB.GetLastPoint().AddComponent<HingeJoint>();
        jointB.connectedBody = hookA.GetLastPoint().GetComponent<Rigidbody>();
        jointB.autoConfigureConnectedAnchor = false;
        jointB.anchor = Vector3.zero;
        jointB.connectedAnchor = new Vector3(0f, -yoffset, 0f);
    //     hookA.GetLastPoint().GetComponent<HingeJoint>().connectedBody = hookB.GetLastPoint().GetComponent<Rigidbody>();
    //     hookB.GetLastPoint().GetComponent<HingeJoint>().connectedBody = hookA.GetLastPoint().GetComponent<Rigidbody>();
    } 

    // Update is called once per frame
    void Update()
    {
        if(hookA.GetLastPoint() != null && hookB.GetLastPoint() != null && !runOnce)
        {
            ConnectLastPointsOfTwoHooks();
            runOnce = true;
        }
    }
}
