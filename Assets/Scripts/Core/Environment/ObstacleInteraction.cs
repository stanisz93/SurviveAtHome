using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;

public enum ObstacleAnimType {slide, vault, climbOverWall};
// this on default should be all, but in more advance options
// like when you want to separately apply different tween moze(different ease) on X and Z
//You could choose different options
public enum AnimAxis {All, XZ, Y};
public enum ConditionType {Equal, Greater, LessThan}
public enum ParamToBeSatisfied {Velocity, DotProdBetweenStartPointAndPlayerDirection}


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
    
    public bool forwardToObstacle = false;

    public Transform posA; // remember that this objects should be directed with Forward toward animation
    public Transform posB;

    public float MoveToPointTime = 0.15f;
    public float MoveToEndPointTime = 0.5f;
    public float ReleaseTime = 1f;


    public ObstacleAnimType animType;
    public Transform edgeStart;
    public Transform edgeEnd;

    public List<Condition> requiredConditions;
    public List<AnimElement> otherAnims;

    private Transform startP;
    private Transform endP; // TODO this is missleading since when you add
    // additional anims its no longer endP


    void OnTriggerEnter(Collider other) {
        ObstacleInteractionManager obs = other.GetComponent<ObstacleInteractionManager>();
        if (obs != null)
        {
            obs.AssignObstacleInteraction(this, true);
        }
    }
    void OnTriggerExit(Collider other) {
        ObstacleInteractionManager obs = other.GetComponent<ObstacleInteractionManager>();
        if (obs != null)
        {
           obs.AssignObstacleInteraction(this, false);
        }
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
                                                Vector3.Dot(player.GetVelocityVector().normalized, startP.forward))
            };
            if(!isSatisfied)
               return false; 
        }
        return true;
    }


    Vector3 FindEdge(Transform player)
    {
        return Vector3.Project((player.position-edgeStart.position),(edgeEnd.position-edgeStart.position))+edgeStart.position;
    }
    public void EstimateSide(Transform player)
    {
        Vector3 closest = FindEdge(player);
        posA.position = posA.position + Vector3.Project(closest - posA.position, posA.right);
        posB.position = posB.position + Vector3.Project(closest - posB.position, posB.right);
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
