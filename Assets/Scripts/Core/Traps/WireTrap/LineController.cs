using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{

    public LayerMask trapAffected;
    public Transform pointA;
    public Transform pointB;
    // Start is called before the first frame update


    private void Start() {
        StartCoroutine(CheckLineInterruption());
    }
    void LineInterruption()
    {
        RaycastHit hit;
        Vector3 dirToTarget = (pointB.position - pointA.position).normalized;
        float dstToTarget = Vector3.Distance(pointA.position, pointB.position);
        if(Physics.Raycast(pointA.position, dirToTarget, out hit, dstToTarget, trapAffected))
        { 
            hit.collider.gameObject.GetComponent<Opponent>().Fall();
            Debug.Log("enemy trapped!");
            Deactivate();
        }

    }
    IEnumerator CheckLineInterruption()
    {
        while(true)
        {
            LineInterruption();
            yield return new WaitForSeconds(0.1f);
        }
    }
    void Deactivate()
    {
        Destroy(gameObject);
    }
}
