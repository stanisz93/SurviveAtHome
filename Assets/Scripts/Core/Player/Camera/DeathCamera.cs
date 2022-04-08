using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathCamera : MonoBehaviour
{
    public LayerMask obstacleMask;
    public List<Transform> potentialPoints;
    public Transform player;
    // Start is called before the first frame update

    // Update is called once per frame



    public void chooseCameraPosition()
    {
        foreach(Transform point in potentialPoints)
        {

            
            Vector3 dirToTarget = (player.position - point.position).normalized;
            float dstToTarget = Vector3.Distance(transform.position, player.position);
            
            if(!Physics.Raycast(point.position, dirToTarget, dstToTarget, obstacleMask))
            {
                transform.position = point.position;
                transform.rotation = point.rotation;
                break;
            } 
        }
    }


}
