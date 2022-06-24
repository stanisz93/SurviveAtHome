using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementMode {Walking, Running, Crouching, Staying};
public enum FightMode {Default, ReceiveDamage};
public enum WeaponType {WoddenStick, Knife, None};

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{   

    public Transform t_mesh;
    public float maxSpeed = 0f;


    [SerializeField]
    private float walkingForwardSpeed = 1.5f;
    [SerializeField]
    private float RunBackwardSpeed = 2f;
    [SerializeField]
    private float walkingSideSpeed = 1.5f;



    private float rotateSpeed = 10f;
    private float smoothSpeed;
    MovementMode movementMode;
    FightMode fightMode = FightMode.Default;

    WeaponType holdMode = WeaponType.None;

    private Rigidbody rigidbody;
    private Vector3 velocity;


    private Character character;
    private OpponentMagnet opponentMagnet;

    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        character = GetComponent<Character>();
        SetMovementMode(MovementMode.Walking);
        opponentMagnet = GetComponentInChildren<OpponentMagnet>();
    }
    
    public Vector3 Velocity {get => rigidbody.velocity; set => velocity = value;}

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
        rigidbody.velocity = Vector3.zero;
    }

    public void ToggleRigidbody() => rigidbody.isKinematic = !rigidbody.isKinematic;
    

    void Update()
    {
        // 
            // if(Vector3.Dot(t_mesh.transform.forward, velocity.normalized) >= 0f)
            // {
        // 
        if (velocity.magnitude > 0f)
        {  
               rigidbody.velocity =  new Vector3(velocity.normalized.x * smoothSpeed, rigidbody.velocity.y, velocity.normalized.z * smoothSpeed);

                // rigidbody.velocity = new Vector3(normVel.x * smoothSpeed.x, rigidbody.velocity.y, normVel.y * smoothSpeed.y);

                smoothSpeed = Mathf.Lerp(smoothSpeed, maxSpeed, Time.deltaTime);
                
                // if (character.opponentFocus == null) // there should be provided some animation to walk around enemy
                // {
               // }
            // t_mesh.rotation = wantedRotation;
                // Opponent attacker = character.GetLatestAttacker();
                // if(attacker != null) // || character.PerformAttackToward()
                // {
                //     Vector3 rotationDir = attacker.transform.position - t_mesh.position;
                //     t_mesh.rotation = Quaternion.LookRotation(rotationDir);
                // }
                // else
                // {
                Quaternion wantedRotation = Quaternion.LookRotation(rigidbody.velocity);
                t_mesh.rotation = Quaternion.Slerp(t_mesh.rotation, wantedRotation, Time.deltaTime * rotateSpeed);
                
                // }
        }
        else
        {
            smoothSpeed = Mathf.Lerp(smoothSpeed, 0, Time.deltaTime);
                
        }
    }

    public void SetFightMode(FightMode mode) { fightMode = mode;}
    public void SetHoldMode(WeaponType mode) { holdMode = mode;}
    public FightMode GetFightMode() {return fightMode;}
    public WeaponType GetHoldMode() {return holdMode;}


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
