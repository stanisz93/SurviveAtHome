using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementMode {Walking, Running, Crouching, Staying};
public enum FightMode {Default, ReceiveDamage};
public enum HoldMode {WoddenStick, Default};

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{   

    public Transform t_mesh;
    public float maxSpeed = 0f;
    public float walkingSpeed = 1.5f;

    private float rotateSpeed = 10f;
    private float smoothSpeed;
    MovementMode movementMode = MovementMode.Staying;
    FightMode fightMode = FightMode.Default;

    HoldMode holdMode = HoldMode.Default;

    private Rigidbody rigidbody;
    private Vector3 velocity;
    private int maxHitAnimSteps = 60;

    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    public Vector3 Velocity {get => rigidbody.velocity; set => velocity = value;}

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
    void Update()
    {
        if (velocity.magnitude > 0)
        {  
            rigidbody.velocity = new Vector3(velocity.normalized.x * smoothSpeed, rigidbody.velocity.y, velocity.normalized.z * smoothSpeed);
        
            smoothSpeed = Mathf.Lerp(smoothSpeed, maxSpeed, Time.deltaTime);
            Quaternion wantedRotation = Quaternion.LookRotation(velocity);
            t_mesh.rotation = Quaternion.Lerp(t_mesh.rotation, wantedRotation, Time.deltaTime * rotateSpeed);
            // t_mesh.rotation = wantedRotation;
        }
        else
        {
            smoothSpeed = Mathf.Lerp(smoothSpeed, 0, Time.deltaTime);
        }
    }

    public void SetFightMode(FightMode mode) { fightMode = mode;}
    public void SetHoldMode(HoldMode mode) { holdMode = mode;}
    public FightMode GetFightMode() {return fightMode;}
    public HoldMode GetHoldMode() {return holdMode;}


    public void SetMovementMode(MovementMode mode)
    {   
        movementMode = mode;
        switch(mode)
        {
            case MovementMode.Walking:
            {
                maxSpeed = walkingSpeed;
                break;
            }
            case MovementMode.Running:
            {
                maxSpeed = 6f;
                break;
            }
            case MovementMode.Crouching:
            {
                maxSpeed = 1f;
                break;
            }
            
        }
    }

    public MovementMode GetMovementMode()
    {
        return movementMode;
    }

}
