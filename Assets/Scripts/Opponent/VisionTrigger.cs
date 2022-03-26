using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "EnemyVision")
        {
            Debug.Log("Enemy found you!");
            Vector3 player_pos = gameObject.transform.position;
            other.transform.parent.GetComponent<Opponent>().Attack(player_pos);
        }
    }   
}
