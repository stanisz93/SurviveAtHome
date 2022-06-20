using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VaultObstacle : MonoBehaviour, IObstacleInteractable
{
    // Start is called before the first frame update

    public ObstacleAnimType animType{ get {return ObstacleAnimType.vault;}}

    public void PerformAction(Animator animator, Character character)
    {
        animator.SetTrigger("Vault");
        animator.SetBool("MirrorAnimation", Random.value > 0.5f); 
                
    }
}