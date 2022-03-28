using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VisionFieldOfView : MonoBehaviour
{
    // Start is called before the first frame update
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public float searchDelay = 0.2f;
    

    public LayerMask visionMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();
    private Opponent opponent;
    private OpponentActions opponentActions;
    private TaskManager taskManager;
    


    void Start()
    {
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
        if (visibleTargets.Count != 0)
            return visibleTargets[0];
        return null;
    }

    void FindVisibleTargets()
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
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal){angleInDegrees += transform.eulerAngles.y;}
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
