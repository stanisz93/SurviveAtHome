using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    // Start is called before the first frame update
    
    private GameObject cam;

    private void Awake() {
        cam = GameObject.FindWithTag("MainCamera");
            
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
