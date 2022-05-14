using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabTrigger : AttackTrigger
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other) {
        
        if(other.transform.tag == "Opponent")
        {
            Debug.Log("Knife area!");
            Vector3 targetDirection = other.gameObject.transform.position - player.position;
            targetDirection = new Vector3(targetDirection.x,  player.position.y, targetDirection.z);
            player.rotation = Quaternion.LookRotation(targetDirection);
            OnHit?.Invoke();
            var opponent = other.gameObject.GetComponent<Opponent>();
            opponent.GotPushed(player, pushForce, pushTime);
            stressReceiver.InduceStress(cameraShake);
        }
    }
}
