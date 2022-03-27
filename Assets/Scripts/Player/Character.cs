using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    private CharacterMovement characterMovement;
    public Camera mainCamera;
    private float forwardInput;
    private float rightInput;
    private int health = 500;


    // Start is called before the first frame update

    // Update is called once per frame

    void Start() {
        characterMovement = GetComponent<CharacterMovement>();
        health = 500;
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

    public void ReduceHealth(int damage)
    {
        health -= damage;
    }
    public int GetHealth(){return health;}
    
    public void AddMovementInput(float forward, float right)
    {
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
                characterMovement.SetMovementMode(mode);
            }
            else
            {
            }
        }
    public MovementMode GetMovement(){return characterMovement.GetMovementMode();}

}
