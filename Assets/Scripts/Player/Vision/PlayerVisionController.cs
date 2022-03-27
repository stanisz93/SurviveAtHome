using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(PlayerVision))]
public class PlayerVisionController : Editor
{
    
    void OnSceneGUI() {
        {
            PlayerVision fov = (PlayerVision)target;
            Handles.color = Color.yellow;
            var pos = fov.transform.position;
            Handles.DrawWireArc(pos, Vector3.up, Vector3.forward, 360, fov.viewRadius);
            Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
            Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);
            Handles.DrawLine(pos, pos + viewAngleA * fov.viewRadius);
            Handles.DrawLine(pos, pos + viewAngleB * fov.viewRadius);

            Handles.color = Color.red;
            foreach (Transform visibleTarget in fov.visibleTargets)
            {
                Handles.DrawLine(pos, visibleTarget.position);
            }
            
        }
    }
}
