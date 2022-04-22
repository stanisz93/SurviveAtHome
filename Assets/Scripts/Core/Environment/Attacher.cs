using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacher : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform attachObj;
    private Collider collider;
    private float maxX;
    private float maxZ;
    private float minX;
    private float minZ;

    private Transform sub;
    void Start()
    {
        attachObj.position = transform.position;
        collider = GetComponent<Collider>();
        Debug.Log($"Collider max: {collider.bounds.max}");
        maxX = collider.bounds.max.x;
        minX = collider.bounds.min.x;
        maxZ = collider.bounds.max.z;
        minZ = collider.bounds.min.z;

    }

    public void SetSubjectToFollow(Transform sub)
    {
        this.sub = sub;
    }

    // Update is called once per frame
    bool BeyondBoundaries(Vector3 pos)
    {
        if(pos.x > maxX || pos.x < minX)
        {
            if (pos.z > maxZ || pos.z < minZ)
            {
                return true;
            }
        }
        return false;
    }
    void Update()
    {
        if(sub)
        {
            var posXZ = new Vector3(this.sub.position.x, transform.position.y, this.sub.position.z);
            var planePosition = Vector3.ProjectOnPlane(posXZ, transform.up) + Vector3.Dot(transform.position, transform.up) * transform.up;
            Debug.DrawLine(transform.position, planePosition);
            if(!BeyondBoundaries(planePosition))
                attachObj.position = planePosition;
        }
    }
}
