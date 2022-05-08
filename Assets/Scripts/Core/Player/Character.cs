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
    private int health;

    private PlayerTriggers playerTriggers;

    public delegate void TriggerAction();

    private Inventory Inventory;

    private Vector3 currrentMouseDirection {get => MouseUtils.MousePositon(mainCamera, characterMovement.t_mesh);}

    private Vector3 globalVelocity;

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


    private void Update() {
        // ShapeUtils.DrawLine(characterMovement.t_mesh.transform.position, characterMovement.t_mesh.transform.position + characterMovement.t_mesh.forward, Color.red);
        var currPos = characterMovement.t_mesh.transform.position;
        ShapeUtils.DrawLine(currPos, currPos + characterMovement.t_mesh.transform.forward, Color.red);
        ShapeUtils.DrawLine(currPos, currPos + characterMovement.Velocity, Color.yellow);
        
    }

    private Vector3 GetVectorRelativeToCamera(float forward, float right)
    {
        Vector3 CamFwd = mainCamera.transform.forward;
        Vector3 CamRight = mainCamera.transform.right;
        Vector3 translation = forward * mainCamera.transform.forward;
        translation += right * mainCamera.transform.right;
        translation.y = 0f;
        Vector3 velocity = Vector3.zero;
        if (translation.magnitude > 0)
        {
            velocity = translation;
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
    


    public void AddMovementInput(float forward, float right, bool rotatePlayer=true)
    {
        if(rotatePlayer)
            characterMovement.t_mesh.rotation = Quaternion.LookRotation(currrentMouseDirection);
        // var localVelocity = GetVectorRelativeToCamera(forward, right);
        
        Vector3 movementRelativeToCamera = GetVectorRelativeToCamera(forward, right);
        characterMovement.Velocity = movementRelativeToCamera;
    }
    public float GetVelocity()
    {
        // Debug.Log(characterMovement.Velocity.magnitude);
        return characterMovement.Velocity.magnitude;
    }
    public float GetVelocityForward()
    {
        // Debug.Log(characterMovement.Velocity.magnitude);
        return Vector3.Dot(characterMovement.Velocity.normalized, characterMovement.t_mesh.forward);

    }
    public float GetVelocityRight()
    {
        return Vector3.Dot(characterMovement.Velocity.normalized, characterMovement.t_mesh.right);
    }
    public FightMode GetFightMode(){return characterMovement.GetFightMode();}

    public MovementMode GetMovement(){return characterMovement.GetMovementMode();}

    public void SetFightmode(FightMode mode){characterMovement.SetFightMode(mode); }

    public void SetToRun(){characterMovement.SetMovementMode(MovementMode.Running);}

    public void SetToCrounch(){characterMovement.SetMovementMode(MovementMode.Crouching);}
    public void SetToWalk(){characterMovement.SetMovementMode(MovementMode.Walking);}

}
