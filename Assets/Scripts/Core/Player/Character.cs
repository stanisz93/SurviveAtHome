using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
[RequireComponent(typeof(Inventory))]
public class Character : MonoBehaviour
{
    private CharacterMovement characterMovement;
    public Camera mainCamera;

    public Camera DeathCamera;
    public HealthBar healthBar;
    public int maxHealth = 500;
    public TriggerAction triggeredAction;
    public float SpeedBeforeKick;

    public GameObject pfBloodEffect;
    public Transform bloodEffectPos;
    private float forwardInput;
    private float rightInput;
    private int health;

    private PlayerTriggers playerTriggers;

    public delegate void TriggerAction();

    private Inventory Inventory;

    // Start is called before the first frame update

    // Update is called once per frame

    void Awake() {
        DeathCamera.enabled = false;
        mainCamera.enabled = true;
        characterMovement = GetComponent<CharacterMovement>();
        playerTriggers = GetComponent<PlayerTriggers>();
        Inventory = GetComponent<Inventory>(); 
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

    public void StartTriggerAction(TriggerAction triggerAction)
    {
        if(playerTriggers.triggerEmpty)
        {
            playerTriggers.triggerEmpty = false;
            triggeredAction = triggerAction;
        }
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
        {
            SetFightmode(FightMode.ReceiveDamage);
            var bloodEffect = Instantiate(pfBloodEffect, bloodEffectPos.position, bloodEffectPos.rotation);
            bloodEffect.GetComponent<ParticleSystem>().Play();
        
        }

    }
    public int GetHealth(){return health;}
    
    public void AddMovementInput(float forward, float right)
    {
        characterMovement.Velocity = AdjustRelativeToCamera(forward, right);
    }
    public float GetVelocity()
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
