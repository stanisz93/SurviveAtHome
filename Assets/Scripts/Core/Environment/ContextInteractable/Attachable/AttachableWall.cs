using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AttachableWall : MonoBehaviour, Attachable
{
    // Start is called before the first frame update
 
  public GameObject pfAttackPlane;
  public Transform createTransform; //to define where it should be created
 private Collider collider;
 float maxZ;
 float minZ;

private Transform plr;
 private GameObject attachablePlane;


 private void Start() {
     plr = GameObject.FindWithTag("PlayerMesh").transform;
     collider = GetComponent<Collider>();
     maxZ = collider.bounds.max.z;
     minZ = collider.bounds.min.z;
     AttachPlane();
     //set up boundaries to expand attachPlane to it

 }
 public void AttachPlane()
 {
        attachablePlane = Instantiate(pfAttackPlane, createTransform.position, transform.rotation);
        attachablePlane.transform.Rotate(0f, 0f, -90f);
        var position = attachablePlane.transform.position;
        attachablePlane.transform.position = new Vector3(position.x, 0.5f, position.z);
        attachablePlane.GetComponent<Attacher>().SetSubjectToFollow(plr);

        /// Here code to extend plane to wall boundaries
 }
}
