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
    public LayerMask terrainMask;
    public Camera DeathCamera;
    public HealthBar healthBar;
    public TriggerAction triggeredAction;
    public float SpeedBeforeKick;
    public Transform leftHand;
    public GameObject pfBloodEffect;
    public Transform bloodEffectPos;

    private Health health;

    private PlayerTriggers playerTriggers;

    public delegate void TriggerAction();

    private Inventory Inventory;

    private Vector3 currrentMouseDirection {get => MouseUtils.MousePositon(mainCamera, characterMovement.t_mesh, terrainMask);}


    // Start is called before the first frame update

    // Update is called once per frame

    void Awake() {
        DeathCamera.enabled = false;
        mainCamera.enabled = true;
        health = GetComponent<Health>();
        characterMovement = GetComponent<CharacterMovement>();
        playerTriggers = GetComponent<PlayerTriggers>();
        Inventory = GetComponent<Inventory>();
        ResetPlayer(); 
        health.OnDamageTake += healthBar.ReduceValue;
        health.OnDamageTake += TakeDamageEffect;
        health.OnDie += playerTriggers.Die;
        }


    public void ResetPlayer()
    {
        transform.position = new Vector3(0.81f, 0.79f, -14.5f);
        healthBar.SetMaxHealth(health.maxHealth);
 
    }

    public bool justDied()
    {
        return playerTriggers.dying;
    }

    public void StartTriggerAction(TriggerAction triggerAction)
    {
        if(playerTriggers.isTriggerEmpty)
        {
            playerTriggers.isTriggerEmpty = false;
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


    public void TakeDamageEffect(int damage)
    {
        SetFightmode(FightMode.ReceiveDamage);
        var bloodEffect = Instantiate(pfBloodEffect, bloodEffectPos.position, bloodEffectPos.rotation);
        bloodEffect.GetComponent<ParticleSystem>().Play();
    }
    

    public void AddMovementInput(float forward, float right, bool rotatePlayer=true)
    {
        var mouseMovement = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));
        if(rotatePlayer && Input.GetMouseButton(1))
        //maybe worth to use here Slerp instead of sudden rotation
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
        var dotVal = Vector3.Dot(characterMovement.Velocity.normalized, characterMovement.t_mesh.forward);
        
        // return dotVal;
        if (Mathf.Abs(dotVal) > 0.5f)
            return dotVal;
        else
            return 0f;
    }
    public float GetVelocityRight()
    {
        var dotVal = Vector3.Dot(characterMovement.Velocity.normalized, characterMovement.t_mesh.right);
        // return dotVal;
        if (Mathf.Abs(dotVal) > 0.5f)
            return dotVal;
        else
            return 0f;
    }
    public FightMode GetFightMode(){return characterMovement.GetFightMode();}

    public MovementMode GetMovement(){return characterMovement.GetMovementMode();}

    public void SetFightmode(FightMode mode){characterMovement.SetFightMode(mode); }

    public void SetToRun(){characterMovement.SetMovementMode(MovementMode.Running);}

    public void SetToCrounch(){characterMovement.SetMovementMode(MovementMode.Crouching);}
    public void SetToWalk(){characterMovement.SetMovementMode(MovementMode.Walking);}

}
