using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VisionFieldOfView : MonoBehaviour
{
    // Start is called before the first frame update
    public float viewRadius = 5f;
    [Range(0,360)]
    public float viewAngle = 150f;
    public float searchDelay = 0.1f;
    public bool visibleAlertBilboard = true;

    public LayerMask visionMask;
    public LayerMask obstacleMask;
    public float maxAlertScore = 100f;
    public float maxCalmingScore = 100f;
    public float calmDownThreshold = 50f;
    public float AlertIncreaseStep = 15;
    public float AlertDecreaseStep = 5;
    public float calmingStep = 4;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public Alert alertUI;
    
    private VisionState state = VisionState.None;
    // private bool suspicious = false;
    private bool isSenseActive = true;

    private Indicator calmIndicator;
    private Indicator alertIndicator;
    
    private Canvas alertBody;
    private Transform playerPos;

public enum VisionState {Founded, Suspicious, None};

    void Start()
    {
        alertBody = alertUI.GetComponentInParent<Canvas>();
        alertBody.enabled = false;
        alertUI.SetDefault(maxAlertScore);
        calmIndicator = new Indicator(maxCalmingScore, maxCalmingScore, calmingStep, calmingStep, false);
        alertIndicator = new Indicator(0f, maxAlertScore, AlertIncreaseStep, AlertDecreaseStep, true);
        StartCoroutine("FindTargetsWithDelay", searchDelay);
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            if(isSenseActive)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
            else
            {
                yield return null;
            }
        }
    }

    public Transform GetPlayerTarget()
    {
        if (state == VisionState.Founded)
            return playerPos;
        return null;
    }

    public bool FoundedObject()
    {
        return state == VisionState.Founded;
    }

    public bool Suspicious()
    {
        return state == VisionState.Suspicious;
    }

    public void ResetSense(float time = 2f)
    {
        calmIndicator.Reset();
        alertIndicator.Reset();
        state = VisionState.None;
        isSenseActive = false;
        alertUI.SetAlertLevel(alertIndicator.GetCurrentValue(), FoundedObject());
        alertBody.enabled = false;
        StartCoroutine(CoolOff(time));
    }


    private void SetStateToFounded()
    {
        state = VisionState.Founded;
        calmIndicator.Activate();
        calmIndicator.SetToZero();
    }

    private void SetStateToSuspicious()
    {
        state = VisionState.Suspicious;
        alertIndicator.SetToMax();
        calmIndicator.Deactivate();
    }

    IEnumerator CoolOff(float time = 2f)
    {
        yield return new WaitForSeconds(time);
        isSenseActive = true;
    }

    void ControlIndicators(bool noticed)
    {
        switch(state)
        {
            case VisionState.Founded:
            {
                if(noticed)
                    calmIndicator.Decrease();
                else
                    calmIndicator.Increase();
                break;
            }
            case VisionState.Suspicious:
            {
                if(noticed)
                    alertIndicator.Increase();
                else
                    alertIndicator.Decrease();
                break;
            }
            case VisionState.None:
            {
                if(noticed)
                    alertIndicator.Increase();
                else
                    alertIndicator.Decrease();
                break;
            }
        }
    }
    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, visionMask);
        for (int i=0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
        if (visibleTargets.Count > 1)
            Debug.LogWarning("There is found more than one object in visible scanner!");
        if(visibleTargets.Count == 1)
        {
            if (isSenseActive)
                alertBody.enabled = true;
            ControlIndicators(true);
            if(state != VisionState.Founded && alertIndicator.ReachedMax())
                {
                    SetStateToFounded();
                }
            playerPos = visibleTargets[0];
        }

        else
        {
            ControlIndicators(false);
            if(calmIndicator.ReachedMax() && calmIndicator.isActive())
                SetStateToSuspicious();

            if(alertIndicator.ReachedMin())
            {
                alertBody.enabled = false;
                state = VisionState.None;
            }
            
        }

        alertUI.SetAlertLevel(alertIndicator.GetCurrentValue(), FoundedObject());


    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal){angleInDegrees += transform.eulerAngles.y;}
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
