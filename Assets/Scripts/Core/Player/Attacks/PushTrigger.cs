using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PushTrigger : MonoBehaviour {


public float kickOpponentReactionDelay = 0.2f;
public float pushDelay = 0.1f;//delay of moving with dween
public float cameraShake = 1f;
public float pushForce = 1f;
public float pushTime = 0.1f;

private StressReceiver stressReceiver;
private Transform player;
private Collider collider;

public Action OnHit;


private void Awake() {
    stressReceiver = GameObject.FindWithTag("MainCamera").GetComponent<StressReceiver>();
    player = GameObject.FindWithTag("PlayerMesh").transform;
    collider = GetComponent<Collider>();
    collider.enabled = false;
}

public void SwitchCollider(bool on)
{
    collider.enabled = on;
}

    private void OnTriggerEnter(Collider other) {
        
        if(other.transform.tag == "Opponent")
        {
            Debug.Log("Enter kick area!");
            Vector3 targetDirection = other.gameObject.transform.position - player.position;
            targetDirection = new Vector3(targetDirection.x,  player.position.y, targetDirection.z);
            player.rotation = Quaternion.LookRotation(targetDirection);
            StartCoroutine(DelayOpponentReaction(other, kickOpponentReactionDelay));
            OnHit?.Invoke();
        }
    }

    private IEnumerator DelayOpponentReaction(Collider other, float delay)
    {
            yield return new WaitForSeconds(delay);
            other.gameObject.GetComponent<Opponent>().GotKicked(player, pushForce, pushTime, pushDelay);
            stressReceiver.InduceStress(cameraShake);

    }
}