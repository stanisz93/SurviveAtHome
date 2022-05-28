using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ragdoll))]

public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
       DrawDefaultInspector();

       Ragdoll ragdoll = (Ragdoll)target;
       if(GUILayout.Button("Apply Force"))
       {
           ragdoll.AddForce();
       }
       if(GUILayout.Button("Toggle ragdoll"))
       {
           ragdoll.ToggleRagdoll();
       }
    }
}