using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HooksConnector : MonoBehaviour
{
    // Start is called before the first frame update
    public CableGenerator hookA;
    public CableGenerator hookB;
    private LineRenderer lr;
    bool runOnce = false;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        
    }

    private void Start() {
            StartCoroutine(RemoveIfbroken());
    }
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

    void SetupLine()
    {
        lr.positionCount = hookA.links + hookB.links + 2;
    }

    // Update is called once per frame
    bool CableIsBroken()
    {
        if (hookA.IsBroken() || hookB.IsBroken())
            return true;
        else
            return false;
    }

    async void Update()
    {
        if(hookA.GetLastPoint() != null && hookB.GetLastPoint() != null && !runOnce)
        {
            ConnectLastPointsOfTwoHooks();
            runOnce = true;
            SetupLine();
        }
        if (runOnce)
        {
            // if(!CableIsBroken())
            // {
                lr.SetPosition(0, hookA.hook.gameObject.transform.position);
                for (int i = 0; i < hookA.links; i++)
                {
                    lr.SetPosition(i + 1, hookA.points[i].position);
                }
                for (int i = hookB.links - 1; i >= 0; i--)
                {
                    lr.SetPosition(hookA.links + hookB.links  - i, hookB.points[i].position);
                }
                lr.SetPosition(hookA.links + hookB.links + 1, hookB.hook.gameObject.transform.position);
            // }
            // else
            // {
            //     Debug.Log("Some logic to render broken line");
            // }
        }
    }

    IEnumerator RemoveIfbroken()
    {
        while(true)
        {
            if(CableIsBroken())
            {   
                yield return new WaitForSeconds(3f);
                Destroy(gameObject);
            }
            else{
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
