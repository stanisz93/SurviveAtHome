using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    private CharacterMovement characterMovement;
    public Camera mainCamera;
    private float forwardInput;
    private float rightInput;


    // Start is called before the first frame update

    // Update is called once per frame

    void Start() {
        characterMovement = GetComponent<CharacterMovement>();
        
    
        }

    private Vector3 AdjustRelativeToCamera(float forward, float right)
    {
        forwardInput = forward;
        rightInput = right;
        Vector3 CamFwd = mainCamera.transform.forward;
        Vector3 CamRight = mainCamera.transform.right;
        Vector3 translation = forward * mainCamera.transform.forward;
        translation += right * mainCamera.transform.right;
        translation.y = 0f;
        Vector3 velocity;
        // forward.y = 0f;
        // right.y = 0f; 
        // forward.Normalize();
        // right.Normalize();
        // Vector3 desireDirection = forward * v.z;
        // desireDirection += right * v.x;
        if (translation.magnitude > 0)
        {
            velocity = translation;
        }
        else
        {
            velocity = Vector3.zero;
        }
        return velocity.normalized;
    }
    public void AddMovementInput(float forward, float right)
    {
        // Vector3 movement = new Vector3(forward, 0f, right);
    //    if (movement.magnitude > 0)
    //         {
    //             velocity = AdjustRelativeToCamera(forward, right);
    //             // movement.Normalize();
    //             Debug.Log(movement.magnitude);
    //             // movement *= characterMovement.speed * Time.deltaTime;
    //             // transform.Translate(movement, Space.World);
    //         }
    //     else
    //     {
    //         velocity = Vector3.zero;
    //     }
        characterMovement.Velocity = AdjustRelativeToCamera(forward, right);

 
    }
    public float getVelocity()
    {
        // Debug.Log(characterMovement.Velocity.magnitude);
        return characterMovement.Velocity.magnitude;
    }

    public void SetToRun()
    {
        ChangeState(MovementMode.Running);
    }
    public void SetToCrounch()
    {
        ChangeState(MovementMode.Crouching);
    }
    public void SetToWalk()
    {
        ChangeState(MovementMode.Walking);
    }
    public void ChangeState(MovementMode mode)
        {
            if (characterMovement.GetMovementMode() != mode)
            {
                // Debug.Log($"State changed! {mode}");
                characterMovement.SetMovementMode(mode);
            }
            else
            {
            }
        }
    public MovementMode GetMovement(){return characterMovement.GetMovementMode();}

}
