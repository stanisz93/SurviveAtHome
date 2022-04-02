using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;        //Public variable to store a reference to the player game object
    private Vector3 offset;            //Private variable to store the offset distance between the player and camera
    // Start is called before the first frame update
    void Start()
    {
       offset = transform.position - player.transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // LateUpdate is called after Update each frame
    void LateUpdate () 
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        Vector3 temp_vec = player.transform.position;
        temp_vec.y = 0f;
        transform.position = temp_vec + offset;
    }
}
