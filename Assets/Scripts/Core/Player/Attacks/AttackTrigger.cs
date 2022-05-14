using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackTrigger : MonoBehaviour {

//delay of moving with dween
public float cameraShake = 1f;
public float pushForce = 1f;
public float pushTime = 0.1f;

public Action OnHit;
private Collider hitTriggerCollider;
protected StressReceiver stressReceiver;
protected Transform player;




private void Awake() {
    stressReceiver = GameObject.FindWithTag("MainCamera").GetComponent<StressReceiver>();
    player = GameObject.FindWithTag("PlayerMesh").transform;
    hitTriggerCollider = GetComponent<Collider>();
    
}

private void Start() {
    GetComponent<Collider>().enabled = false;
}

private void OpponentReaction(Opponent opponent)
{
    DefendItem defendItem = GetComponentInParent<DefendItem>();
    HoldMode weaponMode = HoldMode.Default;
    if (defendItem != null){weaponMode = defendItem.holdMode;}
        switch(weaponMode)
        {
            case(HoldMode.Knife):
            {
                opponent.GotStabbed(player);
                break;
            }
            default:
            {
                opponent.GotPushed(player, pushForce, pushTime);
                opponent.SetKickPos(transform.position);
                break;
            }
        }
}


private void OnTriggerEnter(Collider other) {
        
        Opponent opponent = other.gameObject.GetComponent<Opponent>();
        if(opponent != null)
        {
            Debug.Log("Enter kick area!");
            Vector3 targetDirection = other.gameObject.transform.position - player.position;
            targetDirection = new Vector3(targetDirection.x,  player.position.y, targetDirection.z);
            player.rotation = Quaternion.LookRotation(targetDirection);
            OpponentReaction(opponent);
            OnHit?.Invoke();
            stressReceiver.InduceStress(cameraShake);
        }
    }

}