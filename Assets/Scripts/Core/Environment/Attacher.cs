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

    public void SetPoint(Vector3 coord)
    {
        if(!freezePoint)
        {
            attachObj.transform.position = coord;
            freezePoint = true;
        }
    }

    public void Restart()
    {
        freezePoint = false;
    }

    public Transform GetPointObj()
    {
        return attachObj;
    }

}
