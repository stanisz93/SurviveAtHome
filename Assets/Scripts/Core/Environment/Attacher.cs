using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacher : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform attachObj;
    public Transform plane;
    public Mesh mesh;
    private float maxValue;
    private float minValue;


    private Transform sub;
    public bool freezePoint = false;
    void Start()
    {
        attachObj.position = transform.position;

    }

    public void SetParentBoundary(Mesh mesh)
    {
        maxValue = mesh.bounds.max.z;
        minValue = mesh.bounds.min.z;
    }

    public void SetSubjectToFollow(Transform sub)
    {
        this.sub = sub;
    }

    public void SetPoint()
    {
        freezePoint = true;
    }

    public void Restart()
    {
        freezePoint = false;
    }

    public Transform GetPointObj()
    {
        return attachObj;
    }

    

    // Update is called once per frame
    bool BeyondBoundaries(Vector3 pos)
    {
        // var alongWall = (pos * transform.right);
        // if(alongWall > maxValue || alongWall < minValue)
        // {
        //         return true;
        // }
        return false;
    }
    void Update()
    {
        if(sub && !freezePoint)
        {
            var posXZ = new Vector3(this.sub.position.x, transform.position.y, this.sub.position.z);
            var planePosition = Vector3.ProjectOnPlane(posXZ, transform.right) + Vector3.Dot(transform.position, transform.right) * transform.right;
            
            Debug.DrawLine(transform.position, planePosition);
            if(!BeyondBoundaries(planePosition))
                attachObj.position = planePosition + transform.right * 0.1f;
        }
    }
}
