using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;

public class OpponentMagnet : MonoBehaviour
{
    public float ClosestOpponentLookingPeriodCycle = 0.05f;
    public Transform PlayerMesh;

    public LayerMask opponentMask;
    List <Transform> OpponentsInRadious;

    public Transform MagnetDebug;
    private Transform _nearestOpponent;

    private Character player;
    private SpecialAttacks specialAttacks;
    private ObstacleInteractionManager obstacleInteractionManager;
    private CharacterMovement chrMvmnt;

    public Transform NearestOpponent {get {return _nearestOpponent;}}

    private HashSet<Transform> ToRemove; //Used to recognize those that dies

    private void Start() {
        MagnetDebug.parent = null;
        OpponentsInRadious = new List<Transform>();
        ToRemove = new HashSet<Transform>();
        obstacleInteractionManager = GetComponentInParent<ObstacleInteractionManager>();
        specialAttacks = GetComponentInParent<SpecialAttacks>();
        player = GetComponentInParent<Character>();
        chrMvmnt = GetComponentInParent<CharacterMovement>();
        StartCoroutine(LookForBestFit());
    }
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other) {
        Opponent o = other.GetComponent<Opponent>();
        if(o != null && !ToRemove.Contains(o.transform))
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

    public void RemoveDiedOpponent(Opponent opponent)
{       _nearestOpponent = null;
        ToRemove.Add(opponent.transform);
        if(OpponentsInRadious.Contains(opponent.transform))
        {
            OpponentsInRadious.Remove(opponent.transform);
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
                MagnetDebug.position = transform.position + (new Vector3(closest.position.x, transform.position.y, closest.position.z) - transform.position);
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
            Vector3 movementDir = NearestOpponent.transform.position - PlayerMesh.position;
            movementDir = new Vector3(movementDir.x, 0f, movementDir.z);
            Sequence seq = DOTween.Sequence();
            Vector3 initPlayerRot = player.transform.eulerAngles ;
            Vector3 rot = Quaternion.LookRotation(movementDir.normalized).eulerAngles;
            // maybe here is the problem
            seq.Join(player.transform.DOMove(player.transform.position + movementDir - 0.5f * movementDir.normalized, .4f));
                
            seq.Join(PlayerMesh.transform.DORotate(rot, .4f));
        }
    }

    public void AlternativeNearest()
    {
        RaycastHit hit;
        if (Physics.SphereCast(PlayerMesh.transform.position, 3f, PlayerMesh.transform.forward, out hit, 10f, opponentMask))
        {
            Opponent opponent = hit.collider.transform.GetComponent<Opponent>();
            if(opponent != null)
            {
                _nearestOpponent = hit.collider.transform;
                MagnetDebug.gameObject.SetActive(true);
                MagnetDebug.position = opponent.transform.position;

                if(_nearestOpponent.transform.GetComponent<OpponentActions>().GetOpponentMode() == OpponentMode.Faint)
                {
                    specialAttacks.SetTarget(_nearestOpponent);
                }
                
            }
        }
    }

    IEnumerator LookForBestFit()
    {
        while(true)
            {
                // AlternativeNearest();
                // yield return new WaitForSeconds(ClosestOpponentLookingPeriodCycle);
                _nearestOpponent = FindNearestOpponent();
                if(_nearestOpponent != null)
                    {
                        if(_nearestOpponent.transform.GetComponent<OpponentActions>().GetOpponentMode() == OpponentMode.Faint)
                            specialAttacks.SetTarget(_nearestOpponent);
                    }
                yield return new WaitForSeconds(ClosestOpponentLookingPeriodCycle);
                specialAttacks.RemoveTarget();

                

                
            }
    }


}

