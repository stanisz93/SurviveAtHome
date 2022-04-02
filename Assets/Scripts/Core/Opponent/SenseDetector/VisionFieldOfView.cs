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
    public float increasePoint = 2;
    public float decreasePoint = 2;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public Alert alertUI;
    
    private bool founded = false;
    
    private Opponent opponent;
    private OpponentActions opponentActions;
    private TaskManager taskManager;

    private float currentAlertScore  = 0;
    
    private Canvas alertBody;


    void Start()
    {
        alertBody = alertUI.GetComponentInParent<Canvas>();
        alertUI.SetDefault(maxAlertScore);
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
            return visibleTargets[0];
        return null;
    }

    public bool FoundedObject()
    {
        return founded;
    }



    async void FindVisibleTargets()
    {
        visibleTargets.Clear();
        founded = false;
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

                currentAlertScore += increasePoint;
                alertUI.SetAlertLevel(currentAlertScore);
                if (currentAlertScore >= maxAlertScore)
                {
                    currentAlertScore = maxAlertScore;
                    founded = true;
                }
            }
        else
        {
            currentAlertScore -= decreasePoint;
            if(currentAlertScore <= 0f)
            {
                currentAlertScore = 0f;
                alertBody.enabled = false;
            }
        }


    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal){angleInDegrees += transform.eulerAngles.y;}
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
