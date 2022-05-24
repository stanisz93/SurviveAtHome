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

    void ResetState() // turn off all states
    {
        animator.SetBool("Rushing", false);
        animator.SetBool("Suspicious", false);
        animator.SetBool("Scream", false);
        animator.SetBool("LookAround", false);
    }
        
    void LateUpdate()
    {
        if(animator==null) {Debug.LogWarning("No valid animator!"); return;}
        OpponentMode opponentMode = opponentActions.GetOpponentMode();
        

        switch(opponentMode)
        {
            case OpponentMode.Fall:
            {
                animator.SetBool("Rushing", false);
                animator.SetBool("Suspicious", false);
                break;
            }
            case OpponentMode.Faint:
                ResetState();
                break;
            case OpponentMode.Scream:
            {
                animator.SetBool("Scream", true);
                animator.SetBool("Suspicious", false);
                animator.SetBool("LookAround", false);
                break;
            }
            case OpponentMode.Exploring:
            {
                animator.SetFloat("OpponentVelocity", agent.velocity.magnitude);
                animator.SetBool("Rushing", false);
                animator.SetBool("Suspicious", false);
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
