using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;

public class OpponentMagnet : MonoBehaviour
{
    public float ClosestOpponentLookingPeriodCycle = 0.05f;

    public float maxDistanceToMagnetToOpponent = 0.3f;
    public Transform PlayerMesh;

    public LayerMask opponentMask;
    List <Transform> OpponentsInRadious;

    public Transform targetBall;
    private Opponent _nearestOpponent;

    private Character player;
    private SpecialAttacks specialAttacks;
    private ObstacleInteractionManager obstacleInteractionManager;
    private CharacterMovement chrMvmnt;

    public Opponent NearestOpponent {get {return _nearestOpponent;}}

    private HashSet<Opponent> ToRemove; //Used to recognize those that dies

    private void Start() {
        targetBall.parent = null;
        OpponentsInRadious = new List<Transform>();
        ToRemove = new HashSet<Opponent>();
        obstacleInteractionManager = GetComponentInParent<ObstacleInteractionManager>();
        specialAttacks = GetComponentInParent<SpecialAttacks>();
        player = GetComponentInParent<Character>();
        chrMvmnt = GetComponentInParent<CharacterMovement>();
        StartCoroutine(LookForBestFit());
    }
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other) {
        Opponent o = other.GetComponent<Opponent>();
        if(o != null && !ToRemove.Contains(o))
            OpponentsInRadious.Add(o.zombieMesh);
    }
    private void OnTriggerExit(Collider other) {
        Opponent o = other.GetComponent<Opponent>();
        if(o != null)
        {
            if(specialAttacks.isTarget(o));
                specialAttacks.RemoveTarget();
            OpponentsInRadious.Remove(o.zombieMesh);
        }
    }

    public void RemoveDiedOpponent(Opponent opponent)
{       _nearestOpponent = null;
        ToRemove.Add(opponent);
        if(OpponentsInRadious.Contains(opponent.zombieMesh))
        {
            OpponentsInRadious.Remove(opponent.zombieMesh);
        }
    }
    

    // Update is called once per frame

    // hERE I SHOULD TRY TO TAKE FORWARD OF MESH AND FIND THE OPPONENT THAT PLAYER IS THE MOST DIRECTED TO
    //MAYBE USING THIS YT MOVIE WITH BATMAN
    
    List<Transform> PrepareFinalCandidates()
    {
        List<Transform> finalCandidates = new List<Transform>();
        foreach(Transform o in OpponentsInRadious)
            {
                var zombiePos = o.position;
                Vector3 direction = new Vector3(zombiePos.x, chrMvmnt.t_mesh.position.y, zombiePos.z)  - chrMvmnt.t_mesh.position;
                if (!Physics.Raycast(chrMvmnt.t_mesh.position, direction.normalized, direction.magnitude, 1 << LayerMask.NameToLayer("Obstacles")))
                    finalCandidates.Add(o);
            }
        return finalCandidates;

    }
    Opponent FindNearestOpponent()
    {

        Vector3 stickInput = player.GetTargetInput();
        List<Transform> finalCandidates = PrepareFinalCandidates();
        if (finalCandidates.Count != 0 && stickInput.magnitude > 0)
        { 

             var filtered = finalCandidates.Select(n => new {n, best = Vector3.Dot(stickInput, (new Vector3(n.position.x, PlayerMesh.position.y, n.position.z) - PlayerMesh.position).normalized)})
                            .OrderByDescending( p => p.best)
                            // .Where(p => p.best >= dotThreshold) //now we don't care about it
                            .FirstOrDefault();
            if(filtered == null)
            {
                targetBall.gameObject.SetActive(false);
                return null;
            }
            else
            {
                Opponent closest = filtered.n.GetComponentInParent<Opponent>();
                targetBall.gameObject.SetActive(true);
                closest.GetComponent<CurrentTarget>().SetOpponentAsTarget(targetBall);
                return closest;
            }
}
        else
        {
            targetBall.gameObject.SetActive(false);
            return null;
        }
    }


    // Opponent AlternativeBestFit()
    // {
    //     RaycastHit hit;

    //     // Cast a sphere wrapping character controller 10 meters forward
    //     // to see if it is about to hit anything.
    //     if(player.GetTargetInput().magnitude == 0)
    //         {
    //             return null;
    //             targetBall.gameObject.SetActive(false);
    //         }

    //     Debug.Log($"Stick: {player.GetTargetInput()}");
    //     if (Physics.SphereCast(player.GetHeadPosition(), 3f, player.GetTargetInput(), out hit, 10,  1 << LayerMask.NameToLayer("Opponents")))
    //     {
    //         Debug.Log($"Collide wiht: {hit.collider.transform}");
    //         Opponent o = hit.collider.transform.GetComponent<Opponent>();
    //         if(o != null)
    //         {
                
    //             targetBall.gameObject.SetActive(true);
    //             o.GetComponent<CurrentTarget>().SetOpponentAsTarget(targetBall);
    //             return o;
    //         }
    //         else
    //             targetBall.gameObject.SetActive(false);
    //     }
    //     else
    //         targetBall.gameObject.SetActive(false);
    //     return null;
    // }

    float DistanceFromPlayer() => Vector3.Distance(NearestOpponent.zombieMesh.position, PlayerMesh.position);

    public Vector3 DirectionToOpponent() => (NearestOpponent.zombieMesh.position - new Vector3(PlayerMesh.position.x, NearestOpponent.zombieMesh.position.y, PlayerMesh.position.z)).normalized;

    public void MoveTowardNearestOpponent(float distLeft=0.5f,float moveDelay=0.4f)
    {

        if (NearestOpponent != null && DistanceFromPlayer() <= maxDistanceToMagnetToOpponent)
        {
            obstacleInteractionManager.InterruptMovementSequence();
            Vector3 movementDir = NearestOpponent.zombieMesh.position - PlayerMesh.position;
            movementDir = new Vector3(movementDir.x, 0f, movementDir.z);
            Sequence seq = DOTween.Sequence();
            Vector3 rot = Quaternion.LookRotation(movementDir.normalized).eulerAngles;
            // maybe here is the problem
            if (distLeft != Mathf.Infinity)
                seq.Join(player.transform.DOMove(player.transform.position + movementDir - distLeft * movementDir.normalized, moveDelay));
                
            seq.Join(PlayerMesh.transform.DORotate(rot, .3f));
        }
    }

    public void RotateTowardNearestOpponent(float delay=0.3f)
    {
        Vector3 movementDir = NearestOpponent.zombieMesh.position - PlayerMesh.position;
        Vector3 rot = Quaternion.LookRotation(movementDir.normalized).eulerAngles;
        PlayerMesh.transform.DORotate(rot, delay);
    }


    IEnumerator LookForBestFit()
    {
        while(true)
            {
                // Debug.Log($"Opponent radious size: {OpponentsInRadious.Count}");
                // AlternativeNearest();
                // yield return new WaitForSeconds(ClosestOpponentLookingPeriodCycle);
                _nearestOpponent = FindNearestOpponent();
                if(_nearestOpponent != null)
                    {
                        if(_nearestOpponent.transform.GetComponent<OpponentActions>().GetOpponentMode() == OpponentMode.Faint)
                            specialAttacks.SetTarget(_nearestOpponent);
                    }
                else
                {
                    if (player.IsFightMode())
                        player.SetToDefaultMovement();
                }
                yield return new WaitForSeconds(ClosestOpponentLookingPeriodCycle);
                specialAttacks.RemoveTarget();

                

                
            }
    }


}

