using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTarget : MonoBehaviour
{ //marking enemy that is the closest to the stick direction
    public Transform targetPos;
    // Start is called before the first frame update
    public void SetOpponentAsTarget(Transform targetBall)
    {
        targetBall.position = targetPos.position;
         targetBall.parent = targetPos;
    }

    // Update is called once per frame
}
