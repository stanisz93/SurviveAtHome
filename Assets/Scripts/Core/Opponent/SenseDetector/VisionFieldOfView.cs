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
    

    public LayerMask visionMask;
    public LayerMask obstacleMask;
    public float maxAlertScore = 100f;
    public float maxCalmingScore = 100f;
    public float calmDownThreshold = 50f;
    public float AlertStep = 2;
    public float calmingStep = 2;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public Alert alertUI;
    
    private bool founded = false;
    
    private Opponent opponent;
    private OpponentActions opponentActions;
    private TaskManager taskManager;


    private Indicator calmIndicator;
    private Indicator alertIndicator;
    
    private Canvas alertBody;
    private Transform lastSeen;


    void Start()
    {
        alertBody = alertUI.GetComponentInParent<Canvas>();
        alertUI.SetDefault(maxAlertScore);
        calmIndicator = new Indicator(maxCalmingScore, maxCalmingScore, calmingStep);
        alertIndicator = new Indicator(0f, maxAlertScore, AlertStep);
        StartCoroutine("FindTargetsWithDelay", searchDelay);
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    public Transform GetPlayerTarget()
    {
        if (founded)
            return lastSeen;
        return null;
    }

    public bool FoundedObject()
    {
        return founded;
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
                if (!alertBody.enabled)
                alertBody.enabled = true;

                alertIndicator.Increase();
                if (founded)
                {
                    calmIndicator.Decrease();
                }
                else if(alertIndicator.ReachedMax())
                    {
                        founded = true;
                        calmIndicator.SetToZero();
                    }
                lastSeen = visibleTargets[0];

            }

        else
        {
            if(founded)
            {
                calmIndicator.Increase();
                if(calmIndicator.ReachedMax())
                    founded = false;
            }
            else
                alertIndicator.Decrease();

            if(alertIndicator.ReachedMin() && alertBody.enabled)
                alertBody.enabled = false;
            
        }
        alertUI.SetAlertLevel(alertIndicator.GetCurrentValue(), founded);


    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal){angleInDegrees += transform.eulerAngles.y;}
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
