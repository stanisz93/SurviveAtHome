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
     private OpponentHit opponentHit;

    private void Start() {
        opponent = GetComponent<Opponent>();
        killUtils = GetComponent<KillUtils>();
        vfov = GetComponentInChildren<VisionFieldOfView>();
        animator = GetComponentInChildren<Animator>();
        taskManager = GetComponentInChildren<TaskManager>();
        agent = GetComponent<NavMeshAgent>();
        bonus = GetComponent<HitBonus>();
        ragdoll = GetComponent<Ragdoll>();
        opponentHit = GetComponentInChildren<OpponentHit>();
    }


    void KillingThrow()
    {
            GetComponent<Collider>().enabled = false;
            Vector3 Ragdollforce = targetDirection.normalized * ForceOrRagdollPushWhileThrow;
            // opponent.GetComponent<SpecialKills>().GotKilledByThrow(defendItem.transform, force);
            animator.SetTrigger("DieByThrow");
            StartCoroutine(ragdoll.ToggleRagdollAfter(0.05f, Ragdollforce, rotateForce));
            taskManager.BlockAnyTaskAssigning();
            opponentHit.enabled = false;
            vfov.TurnOffSense();
            Instantiate(pfThrowKillEffect, weapon.position, Quaternion.identity, weapon);
            GameObject stickPointBleeding = new GameObject(); //Instead of weapon
            //that might be destroy and weird thinks happened
            stickPointBleeding.transform.position = weapon.transform.position;
            stickPointBleeding.transform.parent = weapon.transform.parent;
            StartCoroutine(killUtils.InstantiateBloodTexture(bloodTextureThrowDelay, stickPointBleeding.transform, offsetAlongOpponentSpine));
            agent.enabled = false;
            opponent.OnKill?.Invoke(opponent);
            StartCoroutine(killUtils.DestroyOpponentObject(2f));
    }


    public void InvokeReaction(DamageType damageType, WeaponType holdMode, Transform player, float force, float time)
    {
            if(damageType == DamageType.Killing)
                KillingThrow();
    }
}
