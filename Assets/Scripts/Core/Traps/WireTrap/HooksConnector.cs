using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HooksConnector : MonoBehaviour
{
    // Start is called before the first frame update

    private LineRenderer lr;

    private CableGenerator[] hooks;
    bool initialized = false;
    public Transform pointA;
    public Transform pointB;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        hooks = GetComponentsInChildren<CableGenerator>();
        if(hooks.Length != 2)
            Debug.LogError("Two hooks expected!");
        InitializeConnections();
    }


    private void Start() {
            StartCoroutine(RemoveIfbroken());
    }
    void ConnectLastPointsOfTwoHooks()
    {
        GameObject linkA = hooks[0].GetLastPoint();
        float yoffset = linkA.GetComponent<Link>().GetYOffset();
        HingeJoint jointA = linkA.AddComponent<HingeJoint>();
        jointA.connectedBody = hooks[1].GetLastPoint().GetComponent<Rigidbody>();
        jointA.autoConfigureConnectedAnchor = false;
        jointA.anchor = Vector3.zero;
        jointA.connectedAnchor = new Vector3(0f, -yoffset, 0f);

        HingeJoint jointB = hooks[1].GetLastPoint().AddComponent<HingeJoint>();
        jointB.connectedBody = hooks[0].GetLastPoint().GetComponent<Rigidbody>();
        jointB.autoConfigureConnectedAnchor = false;
        jointB.anchor = Vector3.zero;
        jointB.connectedAnchor = new Vector3(0f, -yoffset, 0f);
    //     hooks[0].GetLastPoint().GetComponent<HingeJoint>().connectedBody = hooks[1].GetLastPoint().GetComponent<Rigidbody>();
    //     hooks[1].GetLastPoint().GetComponent<HingeJoint>().connectedBody = hooks[0].GetLastPoint().GetComponent<Rigidbody>();
    }

    void SetupLine()
    {
        lr.positionCount = hooks[0].links + hooks[1].links + 2;
    }

    // Update is called once per frame
    bool CableIsBroken()
    {
        if (hooks[0].IsBroken() || hooks[1].IsBroken())
            return true;
        else
            return false;
    }

    void InitializeConnections()
    {
        
        hooks[0].transform.position = pointA.position;
        hooks[1].transform.position = pointB.position;
        foreach(CableGenerator h in hooks)
            h.GenerateWire();
        if(hooks[0].GetLastPoint() != null && hooks[1].GetLastPoint() != null)
        {
            ConnectLastPointsOfTwoHooks();
            initialized = true;
            SetupLine();
        }
    }

    void Update()
    {
        if (initialized)
        {
            // if(!CableIsBroken())
            // {
                lr.SetPosition(0, hooks[0].hook.gameObject.transform.position);
                for (int i = 0; i < hooks[0].links; i++)
                {
                    lr.SetPosition(i + 1, hooks[0].points[i].position);
                }
                for (int i = hooks[1].links - 1; i >= 0; i--)
                {
                    lr.SetPosition(hooks[0].links + hooks[1].links  - i, hooks[1].points[i].position);
                }
                lr.SetPosition(hooks[0].links + hooks[1].links + 1, hooks[1].hook.gameObject.transform.position);
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
