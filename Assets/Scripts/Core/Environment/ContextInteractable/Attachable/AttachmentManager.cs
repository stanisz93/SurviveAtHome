using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AttachmentManager: BestCandidateManager
{

public IAttachable currentAttachable = null;
public LayerMask TrapDisturbMask;
public Transform pfWireTrap;
public static Action OnSetUp;

private IAttachable AttachObjA, AttachObjB;
private GameObject PointA, PointB;
private bool plantingAllow = false;
private bool firstAttached = false;
public void AttachToObject()
{
    Transform best = GetBestOption();
    if(best != null)
    {
        IAttachable attachable = best.GetComponent<IAttachable>();
        if (attachable == null)
            Debug.LogError("This is not collectible!");
        attachable.AttachPlane();
    }
}

private void OnEnable() {
    OnSetUp = null;
    OnSetUp += AttachFirstPoint;
}

private void OnDisable() {
    OnSetUp -= AttachFirstPoint;
    OnSetUp -= AttachSecondPoint;
}



public void AttachFirstPoint()
{
    if(bestOption != null)
    {
        AttachObjA = bestOption.GetComponent<IAttachable>();
        AttachObjA.PlantItem();
        Transform plantPoint = AttachObjA.GetAttachedPoint();
        PointA = Instantiate(plantPoint.gameObject, plantPoint.position, Quaternion.identity);
        AttachObjA.SwitchAttachedPlane(false);
        firstAttached = true;
        OnSetUp -= AttachFirstPoint;
        OnSetUp += AttachSecondPoint;

    }

}

public void ResetAttachmentProcess()
{
    currentAttachable.Restart();
    if(AttachObjA != null)
        AttachObjA.Restart();
    if(AttachObjB != null)
        AttachObjB.Restart();
    firstAttached = false;
}

void CreateTrap()
{
    Transform g = Instantiate(pfWireTrap);
    g.GetComponent<HooksConnector>().SetWire(PointA.transform, PointB.transform);

}

public void AttachSecondPoint()
{
    if(bestOption != null && plantingAllow)
    {
        AttachObjB = bestOption.GetComponent<IAttachable>();
        AttachObjB.PlantItem();
        Transform plantPoint = AttachObjB.GetAttachedPoint();
        PointB = Instantiate(plantPoint.gameObject, plantPoint.position, Quaternion.identity);
        //Here maybe instantiate trap???
        CreateTrap();
        this.enabled = false;
        
    }

}

public bool CanPlantTrap(Transform potentialSecondPoint)
{
    if (Physics.Linecast(PointA.transform.position, potentialSecondPoint.position, TrapDisturbMask, QueryTriggerInteraction.Ignore))
        return false;
    else
        return true;
}


public override IEnumerator CheckPotentialOptions()
{
        while(true)
        {
            SetBestOption();
            if(bestOption != null)
            {
                IAttachable attachable = bestOption.GetComponent<IAttachable>();
                if(currentAttachable != attachable && !attachable.Forbidden())
                    {
                        if(currentAttachable != null)
                            currentAttachable.SwitchAttachedPlane(false);
                        attachable.SwitchAttachedPlane(true);
                        currentAttachable = attachable;
                    }
            }
            yield return new WaitForSeconds(0.2f);
        }
}
private void Update() {
    
    if(firstAttached)
    {
        if(bestOption != null)
        {   
            var attachedPlane = bestOption.GetComponent<IAttachable>();

            if(!attachedPlane.Forbidden())
            {
                Color color;
                Transform plannedPos = attachedPlane.GetAttachedPoint();
                if(CanPlantTrap(plannedPos))
                {
                    plantingAllow = true;
                    color = Color.green;
                }
                else
                {
                    plantingAllow = false;
                    color = Color.red;
                }

                ShapeUtils.DrawLine(PointA.transform.position,  plannedPos.position, color);
                
            }
        }
    
    }
}

}