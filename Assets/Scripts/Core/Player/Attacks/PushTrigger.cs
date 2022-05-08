using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PushTrigger : MonoBehaviour {

//delay of moving with dween
public float cameraShake = 1f;
public float pushForce = 1f;
public float pushTime = 0.1f;

[HideInInspector] 
public Collider collider;

private StressReceiver stressReceiver;
private Transform player;

public Action OnHit;


private void Awake() {
    stressReceiver = GameObject.FindWithTag("MainCamera").GetComponent<StressReceiver>();
    player = GameObject.FindWithTag("PlayerMesh").transform;
    collider = GetComponent<Collider>();
}

private void Start() {
    collider.enabled = false;
}


private void OnTriggerEnter(Collider other) {
        
        if(other.transform.tag == "Opponent")
        {
            Debug.Log("Enter kick area!");
            Vector3 targetDirection = other.gameObject.transform.position - player.position;
            targetDirection = new Vector3(targetDirection.x,  player.position.y, targetDirection.z);
            player.rotation = Quaternion.LookRotation(targetDirection);
            OnHit?.Invoke();
            var opponent = other.gameObject.GetComponent<Opponent>();
            opponent.SetKickPos(transform.position);
            opponent.GotPushed(player, pushForce, pushTime);
            stressReceiver.InduceStress(cameraShake);
        }
    }

}