using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AttachmentManager: MonoBehaviour
{

    public Transform pfAttacker;

    public LayerMask TrapDisturbMask;
    public LayerMask TerrainMask;
    public Transform pfWireTrap;
    public delegate bool OnAttach();
    public OnAttach onAttach; 



    public IAttachable currentAttachable = null;
    private IAttachable attachObjA, attachObjB = null;
    private Transform pointA, pointB, currentPoint;
    private bool plantingOnePartAllow = false;
    private bool setTrapAllow = false;
    private Transform player; 
    private Camera mainCamera;

    private void OnEnable() {
        // ResetAttachmentProcess();
        onAttach = null;
        onAttach += AttachFirstPoint;

    }

    private void OnDisable() {
        ResetAttachmentProcess();
        onAttach = null;
    }


    private void Awake() {
            player = GameObject.FindWithTag("PlayerMesh").transform;
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            pointA = Instantiate(pfAttacker, Vector3.zero, Quaternion.identity);
            pointB = Instantiate(pfAttacker, Vector3.zero, Quaternion.identity);
    }



    public void CastSetTrapPoint()
    {
        RaycastHit hit;
        Vector3 mouseToPlayerDir = MouseUtils.MousePositon(mainCamera, player, TerrainMask);
        if (Physics.Raycast(player.position, mouseToPlayerDir, out hit, 5f, TrapDisturbMask))
        {
            currentAttachable = hit.collider.gameObject.GetComponent<IAttachable>();
            if(currentAttachable != null)
            {
                if(!currentPoint.gameObject.active)
                    currentPoint.gameObject.SetActive(true);
                plantingOnePartAllow = true;
                Debug.DrawRay(player.position, player.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                currentPoint.position = hit.point;
                currentPoint.position = new Vector3(currentPoint.position.x+ hit.normal.x * 0.1f, 0.5f, currentPoint.position.z + hit.normal.z * 0.1f);
            }
        }
        else
            {
                currentAttachable = null;
                plantingOnePartAllow = false;
                if(currentPoint.gameObject.active)
                    currentPoint.gameObject.SetActive(false);

            }
    }

    public bool AttachFirstPoint()
    {
        if(plantingOnePartAllow)
        {
            attachObjA = currentAttachable;
            currentPoint = pointB;
            onAttach -= AttachFirstPoint;
            onAttach += AttachSecondPoint;
        }
        return false;
    }

    public void ResetAttachmentProcess()
    {
        pointA.gameObject.SetActive(false);
        pointB.gameObject.SetActive(false);
        currentPoint = pointA;
        plantingOnePartAllow = false;
    }

    public bool AttachSecondPoint()
    {
        if(plantingOnePartAllow && setTrapAllow)
        {
            Transform g = Instantiate(pfWireTrap);
            g.GetComponent<HooksConnector>().SetWire(pointA.transform, pointB.transform);
            
            this.enabled = false;
            return true;
        }
        else
            return false;
    }

    public bool CanPlantTrap(Transform potentialSecondPoint)
    {
        if (Physics.Linecast(pointA.transform.position, potentialSecondPoint.position, TrapDisturbMask, QueryTriggerInteraction.Ignore))
            return false;
        else
            return true;
    }

    private void Update() 
    {
        CastSetTrapPoint();
        if(currentPoint == pointB)
        { 
            Color color;
            if(CanPlantTrap(currentPoint) && attachObjA != currentAttachable)
            {
                setTrapAllow = true;
                color = Color.green;
            }
            else
            {
                setTrapAllow = false;
                color = Color.red;
            }

            if(currentPoint.gameObject.active)
                ShapeUtils.DrawLine(pointA.transform.position,  currentPoint.position, color);
                    
        }

    }
}