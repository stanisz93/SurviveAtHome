using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AttachableWall : MonoBehaviour, IAttachable
{
    // Start is called before the first frame update
 
public GameObject pfAttackPlane;
public Transform createTransform; //to define where it should be created
public Collider trigger;
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
     Rigidbody rb = gameObject.AddComponent<Rigidbody>();
     rb.isKinematic = true;
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
        SwitchAttachedPlane(false);
        /// Here code to extend plane to wall boundaries
 }

public void SwitchAttachedPlane(bool on)
{
    attachablePlane.SetActive(on); 
}


public void OnTriggerEnter(Collider other) 
{  
    if(other.transform.tag == "Player")
    {
        var attachPicker = other.gameObject.GetComponent<AttachmentManager>(); /// Here 
        attachPicker.AddPotentialObject(this.transform);
    }
}
public  void OnTriggerExit(Collider other) {
    if(other.transform.tag == "Player")
    {
        var attachPicker = other.gameObject.GetComponent<AttachmentManager>();
        attachPicker.RemovePotentialObject(this.transform);
    }
}


}
