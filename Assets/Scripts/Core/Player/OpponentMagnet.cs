using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
public class OpponentMagnet : MonoBehaviour
{
    public float ClosestOpponentLookingPeriodCycle = 0.1f;
    public Transform PlayerMesh;
    List <Transform> OpponentsInRadious;

    public Transform MagnetDebug;
    private Transform _nearestOpponent;

    private Character player;
    private ObstacleInteractionManager obstacleInteractionManager;
   

    public Transform NearestOpponent {get {return _nearestOpponent;}}

    private void Start() {
        OpponentsInRadious = new List<Transform>();
        obstacleInteractionManager = GetComponentInParent<ObstacleInteractionManager>();
  
        player = GetComponentInParent<Character>();
        StartCoroutine(LookForBestFit());
    }
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other) {
        Opponent o = other.GetComponent<Opponent>();
        if(o != null)
        {
            OpponentsInRadious.Add(o.transform);
        }
    }
    private void OnTriggerExit(Collider other) {
            Opponent o = other.GetComponent<Opponent>();
        if(o != null)
        {
            OpponentsInRadious.Remove(o.transform);
        }
    }

    // Update is called once per frame

    // hERE I SHOULD TRY TO TAKE FORWARD OF MESH AND FIND THE OPPONENT THAT PLAYER IS THE MOST DIRECTED TO
    //MAYBE USING THIS YT MOVIE WITH BATMAN
    Transform FindNearestOpponent(float dotThreshold = 0.4f)
    {
        
        if (OpponentsInRadious.Count != 0)
        { 
            var filtered = OpponentsInRadious.Select(n => new {n, best = Vector3.Dot(PlayerMesh.forward, (new Vector3(n.position.x, PlayerMesh.position.y, n.position.z) - PlayerMesh.position).normalized)})
                            .OrderBy( p => p.best)
                            .Where(p => p.best >= dotThreshold)
                            .FirstOrDefault();
            if(filtered == null)
            {
                MagnetDebug.gameObject.SetActive(false);
                return null;
            }
            else
            {
                Transform closest = filtered.n;
                MagnetDebug.gameObject.SetActive(true);
                MagnetDebug.position = transform.position + (new Vector3(closest.position.x, transform.position.y, closest.position.z) - transform.position).normalized;
                return closest;
            }
        }
        else
        {
            MagnetDebug.gameObject.SetActive(false);
            return null;
        }
    }


    public void MoveTowardNearestOpponent()
    {

        if (NearestOpponent != null)
        {
            obstacleInteractionManager.InterruptMovementSequence();
            Vector3 movementDir = NearestOpponent.position - PlayerMesh.position;
            movementDir = new Vector3(movementDir.x, 0f, movementDir.z);
            Sequence seq = DOTween.Sequence();
            Vector3 initPlayerRot = player.transform.eulerAngles ;
            Vector3 rot = Quaternion.LookRotation(movementDir.normalized).eulerAngles;
            // maybe here is the problem
            seq.Join(player.transform.DOMove(player.transform.position + movementDir - 0.5f * movementDir.normalized, .4f));
                
            seq.Join(PlayerMesh.transform.DORotate(rot, .4f));
        }
    }

    IEnumerator LookForBestFit()
    {
        while(true)
            {
                yield return new WaitForSeconds(ClosestOpponentLookingPeriodCycle);
                _nearestOpponent = FindNearestOpponent();
            }
    }
}

