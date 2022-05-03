using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickTrigger : MonoBehaviour {

public StressReceiver stressReceiver;
public float kickOpponentReactionDelay = 0.2f;
public Transform player;
private Collider collider;

private void Awake() {
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
            
            StartCoroutine(DelayOpponentReaction(other, kickOpponentReactionDelay));
        }
    }

    private IEnumerator DelayOpponentReaction(Collider other, float delay)
    {
            yield return new WaitForSeconds(delay);
            other.gameObject.GetComponent<Opponent>().GotKicked(player);
            stressReceiver.InduceStress(1f);

    }
}