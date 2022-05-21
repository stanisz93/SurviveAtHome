using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public enum ObstacleAnimType {slide, vault};
public class ObstacleInteraction : MonoBehaviour
{
    public Transform posA;
    public Transform posB;

    public float MoveToPointTime = 0.15f;
    public float MoveToEndPointTime = 0.5f;
    public float ReleaseTime = 1f;

    public float interactThreshold = 0.5f;

    public List<Transform> potentialNearestPoints;

    public ObstacleAnimType animType;

    private Transform startP;
    private Transform endP;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other) {
        PlayerTriggers c = other.GetComponent<PlayerTriggers>();
        if (c != null)
        {
            c.AssignObstacleInteraction(this, true);
        }
    }
    void OnTriggerExit(Collider other) {
        PlayerTriggers c = other.GetComponent<PlayerTriggers>();
        if (c != null)
        {
           c.AssignObstacleInteraction(this, false);
        }
    }


    Transform FindNearestPoint(Transform player)
    {
// populate list
       Transform closest =  potentialNearestPoints.Select( n => new { n, distance = Vector3.Distance(player.position, n.position)} )
                    .OrderBy( p => p.distance)
                    .First().n;
        return closest;
    }
    public void EstimateSide(Transform player)
    {
        Transform closest = FindNearestPoint(player);

        posA.position = posA.position + Vector3.Scale(posA.position - closest.position, posA.right);
        posB.position = posB.position + Vector3.Scale(closest.position - posB.position, posB.right);
        if(Vector3.Distance(posA.position, player.position) < Vector3.Distance(posB.position, player.position))
            {
                startP = posA;
                endP = posB;
            }
            else
            {
                startP = posB;
                endP = posA;
            }
    }

        public Transform GetStartPoint()
    {
        return startP;
    }

    public Transform GetEndPoint()
    {
        return endP;
    }

    // Update is called once per frame
}
