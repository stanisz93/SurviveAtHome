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

private Transform plr;
 private GameObject attachablePlane;
 private Attacher attacher;

private bool forbidden=false;





 private void Start() {
     plr = GameObject.FindWithTag("PlayerMesh").transform;
     
     Rigidbody rb = gameObject.AddComponent<Rigidbody>();
     rb.isKinematic = true;
     AttachPlane();
     //set up boundaries to expand attachPlane to it

 }
 public void AttachPlane()
 {
        attachablePlane = Instantiate(pfAttackPlane, createTransform.position, createTransform.rotation);
        var position = attachablePlane.transform.position;
        attachablePlane.transform.position = new Vector3(createTransform.position.x, 0.5f, createTransform.position.z);
        
        attacher = attachablePlane.GetComponent<Attacher>();
        attacher.SetParentBoundary(transform.GetComponent<MeshFilter>().mesh);
        attacher.SetSubjectToFollow(plr);
        SwitchAttachedPlane(false);
        /// Here code to extend plane to wall boundaries
 }

public Transform GetAttachedPoint()
{
    return attacher.GetPointObj();
}


 public void PlantItem()
 {
     attacher.SetPoint();
 }

public void SwitchAttachedPlane(bool on)
{
    attachablePlane.SetActive(on);
}

public bool Forbidden()
{
    if(attacher != null)
        return attacher.freezePoint;
    else
        return forbidden;
}

public void Restart()
{
    attacher.Restart();
    SwitchAttachedPlane(false);
}

public void OnTriggerEnter(Collider other) 
{  
    if(other.transform.tag == "Player")
    {
        var attachPicker = other.gameObject.GetComponent<AttachmentManager>(); /// Here 
        if(attachPicker.enabled)
            attachPicker.AddPotentialObject(this.transform);
    }
}
public  void OnTriggerExit(Collider other) {
    if(other.transform.tag == "Player")
    {
        var attachPicker = other.gameObject.GetComponent<AttachmentManager>();
        if(attachPicker.enabled)
            attachPicker.RemovePotentialObject(this.transform);
    }
}


}
