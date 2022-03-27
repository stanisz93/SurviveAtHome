using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSetPosition : MonoBehaviour
{
    public OpponentNavigationCoroutines coroutineScript;
    public bool debug;
    // Start is called before the first frame update
    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Physics.Raycast(ray, out hit);
        if (hit.collider.gameObject == gameObject)
        {
            Vector3 newTarget = hit.point + new Vector3(0, 0.5f, 0);
            if(debug) Debug.Log($"Clicked at pos {newTarget}"); 
            // This is really work around
            coroutineScript.SetTarget(newTarget);
            coroutineScript.TryToForceTask("Movement", true);
        }
    }
}
