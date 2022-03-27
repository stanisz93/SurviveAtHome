using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject victim;
    void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "EnemyVision")
        {
            Debug.Log("Enemy found you!");
            Vector3 player_pos = gameObject.transform.position;
            // RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
            // if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            // CHECK THIS : https://docs.unity3d.com/ScriptReference/Physics.Raycast.html to run only if player is 
            var opponent = other.transform.parent.GetComponent<Opponent>();
            // oppnentCourutines.TryToForceTask("RushingToAttack", player_pos, true);
            opponent.FindVictim(gameObject);
        }
    }

}
