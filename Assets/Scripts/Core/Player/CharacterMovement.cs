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

    [SerializeField]
    private float walkingForwardSpeed = 1.5f;
    [SerializeField]
    private float walkingBackwardSpeed = 0.5f;
    [SerializeField]
    private float walkingSideSpeed = 1f;

    private float rotateSpeed = 10f;
    private float smoothSpeed;
    MovementMode movementMode = MovementMode.Staying;
    FightMode fightMode = FightMode.Default;

    HoldMode holdMode = HoldMode.Default;

    private Rigidbody rigidbody;
    private Vector3 velocity;

    
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
        // 
            // if(Vector3.Dot(t_mesh.transform.forward, velocity.normalized) >= 0f)
            // {
        // 
        if (velocity.magnitude > 0f)
        {  

                rigidbody.velocity = new Vector3(velocity.normalized.x * smoothSpeed, rigidbody.velocity.y, velocity.normalized.z * smoothSpeed);
                if(Vector3.Dot(t_mesh.transform.forward, velocity.normalized) > 0.7f)
                {
                    smoothSpeed = Mathf.Lerp(smoothSpeed, maxSpeed, Time.deltaTime);
                }
                else
                    smoothSpeed = Mathf.Lerp(smoothSpeed, walkingForwardSpeed, Time.deltaTime);
                var orthogonalMove = Mathf.Abs(Vector3.Dot(t_mesh.transform.right, velocity.normalized)) ;
                if(orthogonalMove > 0.7f)
                    smoothSpeed = Mathf.Lerp(smoothSpeed, 0.1f, Time.deltaTime);
                else if(orthogonalMove > 0.4f)
                    smoothSpeed = Mathf.Lerp(smoothSpeed, walkingForwardSpeed, Time.deltaTime);
                if (!Input.GetMouseButton(1))
                {
                    // Probably worth to use Slerp?
                    Quaternion wantedRotation = Quaternion.LookRotation(velocity);
                    t_mesh.rotation = Quaternion.Lerp(t_mesh.rotation, wantedRotation, Time.deltaTime * rotateSpeed);
                }
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
                maxSpeed = walkingForwardSpeed;
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
