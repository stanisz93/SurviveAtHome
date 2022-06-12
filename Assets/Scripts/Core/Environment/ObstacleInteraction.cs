using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;

public enum ObstacleAnimType {slide, vault, climbOverWall, PushDoor};
// this on default should be all, but in more advance options
// like when you want to separately apply different tween moze(different ease) on X and Z
//You could choose different options
public enum AnimAxis {All, XZ, Y};
public enum ConditionType {Equal, Greater, LessThan}
public enum ParamToBeSatisfied {Velocity, DotProdBetweenStartPointAndPlayerDirection, DotBetweenForward}


    [Serializable]
    public struct AnimElement {
      public Transform obj;
      public AnimAxis animAxis;
      public float duration;
    public bool isJoinedWithPrev; // if true then it will be append to previous anim
    public AnimationCurve animCurve;
    }

    [Serializable]
    public struct Condition {
      public ConditionType conditionType;
      public ParamToBeSatisfied param;
      public float value;

    }



public class ObstacleInteraction : MonoBehaviour
{
    public float timeScaleWhenPerform = 1f;
    public bool StepDebug = false;

    //Sometimes interaction with object require only moving to that object and run animation
    public bool hasEndPosition = true;

    
    public bool forwardToObstacle = false;

    public Transform posA; // remember that this objects should be directed with Forward toward animation
    public Transform posB;

    public float MoveToPointTime = 0.15f;
    public float MoveToEndPointTime = 0.5f;
    public float ReleaseTime = 1f;


//In general this part with force should be move to some
//extended version of interactive object, like DamagableInteractive etc.
//if needed
    public float forceMultiplier = 100f;

    public float pushWhenHit = 3f;
    public float timeOfPushing = 1f;

    // Time when to stop create effect
    public float forceEffectTimeDecay = 0.1f;


    public ObstacleAnimType animType;
    public Transform edgeStart;
    public Transform edgeEnd;

    public List<Condition> requiredConditions;
    public List<AnimElement> otherAnims;

    private Transform startP;
    private Transform endP; // TODO this is missleading since when you add
    // additional anims its no longer endP
    private bool isForceApplied = false;

    void OnTriggerEnter(Collider other) {
        ObstacleInteractionManager obs = other.GetComponent<ObstacleInteractionManager>();
        if (obs != null)
        {
            obs.AssignObstacleInteraction(this, true);
        }
        Opponent opponent = other.GetComponent<Opponent>();
        if(opponent != null && isForceApplied)
        {
            opponent.GetComponent<MeleeReaction>().InvokeReaction(DamageType.ToTheGround, WeaponType.None, transform, pushWhenHit, timeOfPushing);
            //opponent.GotPushed();
        }
    }
    void OnTriggerExit(Collider other) {
        ObstacleInteractionManager obs = other.GetComponent<ObstacleInteractionManager>();
        if (obs != null)
        {
           obs.AssignObstacleInteraction(this, false);
        }
    }

    public void ApplyForce(Vector3 force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            isForceApplied = true;
            rb.AddForce(force * forceMultiplier, ForceMode.Impulse);
            TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
            tr.enabled = true;
            StartCoroutine(ForceEffect(tr));
        }
    }

    IEnumerator ForceEffect(TrailRenderer tr)
    {
        yield return new WaitForSeconds(forceEffectTimeDecay);
        isForceApplied = false;
        tr.enabled = false;

    }

    bool IsRequirementSatisfied(Condition condition, float value)
    {
        return condition.conditionType switch
        {
            ConditionType.Equal => value == condition.value,
            ConditionType.Greater => value > condition.value,
            ConditionType.LessThan => value < condition.value
        };
    }

    public bool AreConditionsSatisfied(Character player)
    {
        foreach(var condition in requiredConditions)
        {
            bool isSatisfied = condition.param switch
            {
                ParamToBeSatisfied.Velocity => IsRequirementSatisfied(condition, player.GetVelocityMagnitude()),
                ParamToBeSatisfied.DotProdBetweenStartPointAndPlayerDirection => IsRequirementSatisfied(condition, 
                                                Vector3.Dot(player.GetVelocityVector().normalized, startP.forward)),
                ParamToBeSatisfied.DotBetweenForward =>  IsRequirementSatisfied(condition, 
                                                Vector3.Dot(player.GetForwardDirection(), startP.forward))

            };
            if(!isSatisfied)
               return false; 
        }
        return true;
    }


//this method assume that there is and invicible line that project specific point
//when player is the closest
//like vault over some long object, it will vault over the closest point on that object
// Point of acces should be perpendicular to start and end point
    Vector3 FindEdge(Transform player)
    {
        if(edgeStart != null && edgeEnd != null)
            return Vector3.Project((player.position-edgeStart.position),(edgeEnd.position-edgeStart.position))+edgeStart.position;
        else return Vector3.zero;
    }
    public void EstimateSide(Transform player)
    {
        Vector3 closest = FindEdge(player);

        if(closest != Vector3.zero)
        {
            posA.position = posA.position + Vector3.Project(closest - posA.position, posA.right);
            posB.position = posB.position + Vector3.Project(closest - posB.position, posB.right);
        }
        if (otherAnims != null)
        {
            foreach(var pos in otherAnims)
            {
                // we don't won't to double translate for the same object
                if(!pos.isJoinedWithPrev)
                    pos.obj.position = pos.obj.position + Vector3.Project(closest - pos.obj.position, pos.obj.right);
            }
        }
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


    Tween GetSpecificTween(Transform player, AnimElement animElement)
    {
        float duration = animElement.duration;
        Vector3 position = animElement.obj.position;
        Tween tween = animElement.animAxis switch
        {
            AnimAxis.XZ => DOTween.Sequence()
                            .Append(player.DOMoveX(position.x, duration))
                            .Join(player.DOMoveZ(position.z, duration)),
            AnimAxis.Y => player.DOMoveY(position.y, duration),
            AnimAxis.All => player.DOMove(position, duration),

        };
        // This seems to be problematic, to be checked
        //  if(animElement.animCurve != null)
        //     return tween.SetEase(animElement.animCurve);
        return tween;
  
    }

    public Sequence RunSequence(Transform player)
    {
        Sequence moveSequence = DOTween.Sequence();
        moveSequence.AppendCallback(() => Time.timeScale = timeScaleWhenPerform);

            moveSequence.Append(player.DOMove(startP.position, MoveToPointTime));
        
        // Not required, just move to StartPoint a could be enough
        if(hasEndPosition)
            moveSequence.Append(player.DOMove(endP.position, MoveToEndPointTime));
        if (otherAnims != null)
        {
            foreach(var otherAnim in otherAnims)
            {

                if(otherAnim.isJoinedWithPrev)
                    moveSequence.Join(GetSpecificTween(player, otherAnim));
                else
                { 
                    if (StepDebug) //only without isJoinedWithPrev, because join will be connected with this callback
                    moveSequence.AppendCallback(() => Debug.Log($"Moving To: {otherAnim.obj.gameObject.name}"));
                    moveSequence.Append(GetSpecificTween(player, otherAnim));
                }

            }
        }
        moveSequence.AppendCallback(() => Time.timeScale = 1f);
        return moveSequence;   
    }

    public Transform GetEndPoint()
    {
        return endP;
    }

    // Update is called once per frame
}
