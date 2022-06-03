using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DistanceAttackReaction : MonoBehaviour, IOpponentReaction

{

    public GameObject pfThrowKillEffect;

    public float ForceOrRagdollPushWhileThrow = 1f;
    public float bloodTextureThrowDelay = 0.2f; // texture for throwing
    public float offsetAlongOpponentSpine = 0.0f;

    public float rotateForce = 1f;
    public Vector3 targetDirection {set; get;}
    public HitBonus bonus {set; get;}
    public Transform weapon {set; get;}
    private KillUtils killUtils;
    private Animator animator;
    private VisionFieldOfView vfov;
     private Opponent opponent;
     private TaskManager taskManager;
     private Ragdoll ragdoll;
     private NavMeshAgent agent;

    private void Start() {
        opponent = GetComponent<Opponent>();
        killUtils = GetComponent<KillUtils>();
        vfov = GetComponentInChildren<VisionFieldOfView>();
        animator = GetComponentInChildren<Animator>();
        taskManager = GetComponentInChildren<TaskManager>();
        agent = GetComponent<NavMeshAgent>();
        bonus = GetComponent<HitBonus>();
        ragdoll = GetComponent<Ragdoll>();
    }



    public void InvokeReaction(WeaponType holdMode, Transform player, float force, float time)
    {
            // defendItem.transform.parent = opponent.transform;
            GetComponent<Collider>().enabled = false;
            Vector3 Ragdollforce = targetDirection.normalized * ForceOrRagdollPushWhileThrow;
            // opponent.GetComponent<SpecialKills>().GotKilledByThrow(defendItem.transform, force);
            animator.SetTrigger("DieByThrow");
            StartCoroutine(ragdoll.ToggleRagdollAfter(0.05f, Ragdollforce, rotateForce));
            taskManager.BlockAnyTaskAssigning();
            vfov.TurnOffSense();
            Instantiate(pfThrowKillEffect, weapon.position, Quaternion.identity, weapon);
            StartCoroutine(killUtils.InstantiateBloodTexture(bloodTextureThrowDelay, weapon, offsetAlongOpponentSpine));
            agent.enabled = false;
            StartCoroutine(killUtils.DestroyOpponentObject(2f));
    }
}
