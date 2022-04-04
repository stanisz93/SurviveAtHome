using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class Character : MonoBehaviour
{
    private CharacterMovement characterMovement;
    public Camera mainCamera;
    public HealthBar healthBar;
    public int maxHealth = 500;
    private float forwardInput;
    private float rightInput;

    
    private int health;
    private Slidable slidable;
    public bool isSliding = false;
    public Slidable Slidable { get => slidable; set => slidable = value; }


    // Start is called before the first frame update

    // Update is called once per frame

    void Start() {
        characterMovement = GetComponent<CharacterMovement>();
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
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

    public void SetToSlide(){characterMovement.SetMovementMode(MovementMode.Sliding);}

    public void SetToCrounch(){characterMovement.SetMovementMode(MovementMode.Crouching);}
    public void SetToWalk(){characterMovement.SetMovementMode(MovementMode.Walking);}

    public void Slide()
    {
        isSliding = true;
        Vector3 t_slidePoint = slidable.GetSlidePoint();
        Vector3 t_landpoint = slidable.GetEndSlidePoint();
        Sequence sq = DOTween.Sequence();
        sq.Append(transform.DOMove(t_slidePoint, .1f));
        sq.Append(transform.DOMove(t_landpoint, .8f));
        sq.AppendCallback(()=> isSliding = false);
        Slidable = null;
    }
}   
