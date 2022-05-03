using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OpponentAnimationController : MonoBehaviour
{

    public Animator animator;

    private NavMeshAgent agent;
    private OpponentActions opponentActions;
    private Opponent opponent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        opponentActions = GetComponent<OpponentActions>();
        opponent = GetComponent<Opponent>();
        
    }
    void LateUpdate()
    {
        if(animator==null) {Debug.LogWarning("No valid animator!"); return;}
        OpponentMode opponentMode = opponentActions.GetOpponentMode();
        

        switch(opponentMode)
        {
            case OpponentMode.beingKicked:
            {
                animator.SetBool("Rushing", false);
                animator.SetBool("HitPlayer", false);

                break;
            }
            case OpponentMode.Agonize:
            {
                animator.SetBool("Agonize", true);
                animator.SetBool("Suspicious", false);
                break;
            }
            case OpponentMode.Fall:
            {
                animator.SetBool("Rushing", false);
                animator.SetBool("Suspicious", false);
                break;
            }
            case OpponentMode.Scream:
            {
                animator.SetBool("Scream", true);
                animator.SetBool("Agonize", false);
                animator.SetBool("Suspicious", false);
                break;
            }
            case OpponentMode.Exploring:
            {
                animator.SetBool("Agonize", false);
                animator.SetFloat("OpponentVelocity", agent.velocity.magnitude);
                animator.SetBool("Rushing", false);
                animator.SetBool("Suspicious", false);
                animator.SetBool("HitPlayer", false);
                break;
            }
            case OpponentMode.LookAround:
            {
                animator.SetBool("Suspicious", false);
                break;
            }
            case OpponentMode.Rushing:
            {
                animator.SetBool("Scream", false);
                animator.SetBool("Rushing", true);
                animator.SetBool("HitPlayer", false);

                break;
            }
            case OpponentMode.HitPlayer:
            {
                animator.SetBool("HitPlayer", true);
                 animator.SetBool("Rushing", false);
                break;
            }
            case OpponentMode.Checking:
            {
                animator.SetBool("Suspicious", true);
                animator.SetBool("Rushing", false);
                break;
            }
            case OpponentMode.Attacking:
            {
                animator.SetBool("Rushing", false);
                break;
            }
        
        }
            
    }
}
