using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    private CharacterMovement characterMovement;
    public Camera mainCamera;
    public HealthBar healthBar;
    public int maxHealth = 500;
    private float forwardInput;
    private float rightInput;

    private int health;

    private PlayerTriggers playerTriggers;
    public delegate void TriggerAction();
    public TriggerAction triggeredAction;

    // Start is called before the first frame update

    // Update is called once per frame

    void Start() {
        characterMovement = GetComponent<CharacterMovement>();
        playerTriggers = GetComponent<PlayerTriggers>();
        ResetPlayer();
        }

    public void ResetPlayer()
    {
        transform.position = new Vector3(0.81f, 0.79f, -14.5f);
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public bool justDied()
    {
        return playerTriggers.dying;
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
        healthBar.SetHealth(health);
        if (health < 0 && !playerTriggers.dying)
        {
            triggeredAction = playerTriggers.Die;
        }
        else
            SetFightmode(FightMode.ReceiveDamage);

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
    public FightMode GetFightMode(){return characterMovement.GetFightMode();}

    public MovementMode GetMovement(){return characterMovement.GetMovementMode();}

    public void SetFightmode(FightMode mode){characterMovement.SetFightMode(mode); }

    public void SetToRun(){characterMovement.SetMovementMode(MovementMode.Running);}

    public void SetToCrounch(){characterMovement.SetMovementMode(MovementMode.Crouching);}
    public void SetToWalk(){characterMovement.SetMovementMode(MovementMode.Walking);}

}
