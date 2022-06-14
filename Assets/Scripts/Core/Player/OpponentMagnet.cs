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
    List <Opponent> OpponentsInRadious;

    public Transform MagnetDebug;
    private Opponent _nearestOpponent;

    private Character player;
    private SpecialAttacks specialAttacks;
    private ObstacleInteractionManager obstacleInteractionManager;
    private CharacterMovement chrMvmnt;

    public Opponent NearestOpponent {get {return _nearestOpponent;}}

    private HashSet<Opponent> ToRemove; //Used to recognize those that dies

    private void Start() {
        MagnetDebug.parent = null;
        OpponentsInRadious = new List<Opponent>();
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
        {
            RaycastHit hit;
            var zombiePos = o.zombieMesh.position;
            Vector3 direction = chrMvmnt.t_mesh.position - new Vector3(zombiePos.x, chrMvmnt.t_mesh.position.y, zombiePos.z);
            if (!Physics.Raycast(transform.position, direction, out hit, 5f, 1 << LayerMask.NameToLayer("Obstacles")))
            {
                OpponentsInRadious.Add(o);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        Opponent o = other.GetComponent<Opponent>();
        if(o != null)
        {
            if(specialAttacks.isTarget(o));
                specialAttacks.RemoveTarget();
            OpponentsInRadious.Remove(o);
        }
    }

    public void RemoveDiedOpponent(Opponent opponent)
{       _nearestOpponent = null;
        ToRemove.Add(opponent);
        if(OpponentsInRadious.Contains(opponent))
        {
            OpponentsInRadious.Remove(opponent);
        }
    }
    

    // Update is called once per frame

    // hERE I SHOULD TRY TO TAKE FORWARD OF MESH AND FIND THE OPPONENT THAT PLAYER IS THE MOST DIRECTED TO
    //MAYBE USING THIS YT MOVIE WITH BATMAN
    Opponent FindNearestOpponent(float dotThreshold = 0.4f)
    {
        
        if (OpponentsInRadious.Count != 0)
        { 
            var filtered = OpponentsInRadious.Select(n => new {n, best = Vector3.Dot(chrMvmnt.Velocity.normalized, (new Vector3(n.transform.position.x, 0f, n.transform.position.z) - PlayerMesh.position).normalized)})
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
                Opponent closest = filtered.n;
                MagnetDebug.gameObject.SetActive(true);
                MagnetDebug.position = transform.position + (new Vector3(closest.transform.position.x, transform.position.y, closest.transform.position.z) - transform.position);
                return closest;
            }
        }
        else
        {
            MagnetDebug.gameObject.SetActive(false);
            return null;
        }
    }


    public void MoveTowardNearestOpponent(float distLeft=0.5f,float moveDelay=0.4f)
    {

        if (NearestOpponent != null)
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

