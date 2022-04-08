using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OpponentAnimationController : MonoBehaviour
{

    public Animator animator;

    private NavMeshAgent agent;
    private OpponentActions opponentActions;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        opponentActions = GetComponent<OpponentActions>();
        
    }
    void LateUpdate()
    {
        if(animator==null) {Debug.LogWarning("No valid animator!"); return;}
        OpponentMode opponentMode = opponentActions.GetOpponentMode();
        switch(opponentMode)
        {
            case OpponentMode.Agonize:
            {
                animator.SetBool("Agonize", true);
                animator.SetBool("Suspicious", false);
                break;
            }
            case OpponentMode.Exploring:
            {
                animator.SetBool("Agonize", false);
                animator.SetFloat("OpponentVelocity", agent.velocity.magnitude);
                animator.SetBool("Rushing", false);
                animator.SetBool("Attack", false);
                animator.SetBool("Suspicious", false);
                break;
            }
            case OpponentMode.Rushing:
            {
                animator.SetBool("Agonize", false);
                animator.SetBool("Rushing", true);
                animator.SetBool("Attack", false);
                animator.SetBool("Suspicious", false);
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
                animator.SetBool("Attack", true);
                animator.SetBool("Rushing", false);
                break;
            }
        
        }
    }
}
