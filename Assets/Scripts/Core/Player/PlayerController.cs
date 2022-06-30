using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;


public enum ControllerMode {DefaultMovement, Inventory, SettingTrap, Aiming, UnderBed};

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterMovement))]
public class PlayerController : MonoBehaviour
{

    public LayerMask terrainMask;
  

    private Character character;
    private CharacterMovement chrMvmnt;
    private CapsuleCollider collider;

    private PlayerTriggers playerTriggers;
    private ItemPickupManager itemPickupManager;

    private AttachmentManager attachmentManager;

    private ControllerMode controllerMode;

    private float useClickTimeWindow = 0.2f; //after this value crafting is used
    private float startClick;
    private bool mouseIsClicked = false;
    private bool craftProcess = false;
    private ItemTile highlightedTile = null;

    public bool blockMovement = false;
    private PlayerAnimationController playerAnimationController;
    private InventoryUI inventoryUI;
    private Transform characterMesh;
    private Camera mainCamera;

    private HitBonus hitBonus;
    private PlayerAnimatorEventController playerAnimatorEventController;
    private ObstacleInteractionManager obstacleInteractionManager;
    private InteractionBonus interactionBonus;
    private SpecialAttacks specialAttacks;
    private AttackTriggersManager attackTriggersManager;

    private Endurance endurance;

    private PlayerActions controls;

    private PlayerInput playerInput;



    void Awake(){
        character = GetComponent<Character>();
        collider = GetComponent<CapsuleCollider>();
        chrMvmnt = GetComponent<CharacterMovement>();
        playerTriggers = GetComponent<PlayerTriggers>();
        obstacleInteractionManager = GetComponent<ObstacleInteractionManager>();
        interactionBonus = GetComponentInChildren<InteractionBonus>();
        playerAnimationController = GetComponent<PlayerAnimationController>(); 
        itemPickupManager = GetComponent<ItemPickupManager>();
        attachmentManager = GetComponent<AttachmentManager>();
        attachmentManager.enabled = false;
        inventoryUI =  GetComponentInChildren<InventoryUI>();
        characterMesh = GameObject.FindWithTag("PlayerMesh").transform;
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        hitBonus = GetComponent<HitBonus>();
        playerAnimatorEventController = GetComponentInChildren<PlayerAnimatorEventController>();
        specialAttacks = GetComponentInChildren<SpecialAttacks>();
        attackTriggersManager = GetComponentInChildren<AttackTriggersManager>();
        endurance = GetComponent<Endurance>();

        playerInput = GetComponent<PlayerInput>();
        controls = new PlayerActions();
        controls.DefaultMovement.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        controls.DefaultMovement.Targeting.performed += ctx => Targeting(ctx.ReadValue<Vector2>());
        controls.DefaultMovement.Movement.canceled += ctx => Move(Vector2.zero);
        controls.DefaultMovement.Run.performed += ctx => StartRunning();
        controls.DefaultMovement.Run.canceled += ctx => StartCoroutine(WaitForSecondsBeforeWalk());
        controls.DefaultMovement.Interaction.performed += ctx => OnInteraction();
        controls.DefaultMovement.Dodge.performed += ctx => {
            if(character.IsFightMode()) character.StartTriggerAction(playerTriggers.Dodge);
        };
        // controls.DefaultMovement.Aiming.performed += ctx => OnStartAiming();
        controls.DefaultMovement.Crouch.performed += ctx => character.ToggleCrouch();
        controls.DefaultMovement.PickupItem.performed += ctx => OnPickupItem();
        controls.DefaultMovement.Attack.performed += ctx => OnAttack();
        controls.DefaultMovement.Inventory.performed += ctx => OnInventoryEnter();

        controls.DefaultMovement.Throw.performed += ctx => OnReleaseThrow();
        

        

        //Aiming
        // controls.Aiming.StopAiming.canceled += ctx => OnStopAiming();
        // //for PC
        // // controls.Aiming.AimingDirection.performed += ctx => RotatePlayerWithRespectToMouse(ctx.ReadValue<Vector2>());
        // //FOR Console
        // controls.Aiming.AimingDirection.performed += ctx => Targeting(ctx.ReadValue<Vector2>());
        // controls.Aiming.ContinueAiming.performed += ctx => StartCoroutine(playerTriggers.ContinueAimToThrow());
        // controls.Aiming.ContinueAiming.canceled += ctx => StartCoroutine(OnReleaseThrow());
        
        /// Special cases
        controls.UnderBed.EndAction.performed += ctx => StartCoroutine(OnLeftBed());

        //Inventory
        controls.Inventory.LeftInventory.performed += ctx => {
            controls.Inventory.Disable();
            controls.DefaultMovement.Enable();
            inventoryUI.LeftInventory();
        };

    }

    // Start is called before the first frame updat

    IEnumerator OnLeftBed()
    {
        // SwitchActionMap(ControllerMode.DefaultMovement);
        Debug.Log("Enter LEft bed");
        yield return playerTriggers.LeftBed();
        character.ToogleKinematic();
        controls.DefaultMovement.Enable();
        controls.UnderBed.Disable();

    }

    public void RotateWhileAiming(Vector2 stickValue)
    {
        Vector3 dir = character.GetVectorRelativeToCamera(stickValue.y, stickValue.x);
        Quaternion wantedRotation = Quaternion.LookRotation(dir);
        characterMesh.rotation = wantedRotation;
    }

    public void TurnOnBedMap()
    {
        controls.DefaultMovement.Disable();
        controls.UnderBed.Enable();
    }

    IEnumerator CraftProcess(ItemTile tile)
    {
        yield return new WaitForSeconds(useClickTimeWindow);
        if(!mouseIsClicked)
            yield break;
        craftProcess = true;
        Coroutine craftCoroutine = StartCoroutine(tile.TryToCraft());

        while(mouseIsClicked)
        {
            yield return null;
        }
        if (craftCoroutine != null)
            {
                StopCoroutine(craftCoroutine);
                tile.ResetCraftProgress();
            }
        craftProcess = false;

    }

    void Move(Vector2 move)
    {
        if(blockMovement)
            character.AddMovementInput(0f, 0f);
        else
            {
                character.AddMovementInput(move.y, move.x);

            }
        
    }

    void Targeting(Vector2 target)
    {
        character.AddTargetInput(target.y, target.x);
    }

    private void OnEnable()
    {
        controls.DefaultMovement.Enable();
    }
    private void OnDisable()
    {
         controls.DefaultMovement.Disable();
    }

    void StartRunning()
    {
            if (endurance.GetCurrentEndurance() >= character.energyConsumption.runningPerFrame)
            {
                    character.SetToRun();
            }
            else
            {
                character.SetToWalk();
            }
    }

    IEnumerator WaitForSecondsBeforeWalk()
    {
        yield return new WaitUntil(() => character.GetVelocityMagnitude() < 2f);
        character.SetToWalk();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (!playerTriggers.dying)
    //     {



    //         // if (controllerMode == ControllerMode.DefaultMovement)
    //         // {
    //         //     ManageNormalControl();
    //         // }
    //         if(controllerMode == ControllerMode.Inventory)
    //         {   
                
    //             // ManageInventory();

    //         }
    //         else if (controllerMode == ControllerMode.SettingTrap)
    //         {
    //             ManageSettingTrapControl();
    //         }
    //         // else if(controllerMode == ControllerMode.Aiming)
    //         // {
    //         //     ManagerThrowControl();
    //         // }
    //         // else if(controllerMode == ControllerMode.UnderBed)
    //         // {
    //         //     if(playerTriggers.IsTriggerEmpty())
    //         //        StartCoroutine(ManageUnderBedControl());
    //         // }
    //     }
    // }


    // void ManageInventory()
    // {
    //     var anyTile = inventoryUI.ContainsAnyTile(Input.mousePosition);
    //     if(anyTile is ItemTile)
    //         highlightedTile = (ItemTile)anyTile;
    //     if(Input.GetMouseButtonDown(0) && highlightedTile != null && !craftProcess)
    //     {
    //         startClick = Time.time;
    //         mouseIsClicked = true;
    //         StartCoroutine(CraftProcess(highlightedTile));
    //     }
    //     if(Input.GetMouseButtonUp(0))
    //     {
    //         mouseIsClicked = false;
    //         if(!craftProcess)
    //         {
    //             Debug.Log("Use item method!");
    //             inventoryUI.LeftInventory();
    //             controllerMode = ControllerMode.SettingTrap;
    //             attachmentManager.enabled = true;
    //         }
    //     }
    //     if(!Input.GetKey(KeyCode.Tab))
    //     {
            
    //         inventoryUI.LeftInventory();
    //         controllerMode = ControllerMode.DefaultMovement;
    //     }
    // }

    void RotatePlayerWithRespectToMouse(Vector2 mousePos)
    {
        Debug.Log($"Mouse POS: {mousePos}");
        Vector3 mouseToPlayerDir = MouseUtils.MousePositonRelativeToCamera(mousePos, mainCamera, chrMvmnt.t_mesh, terrainMask);
        Quaternion wantedRotation = Quaternion.LookRotation(mouseToPlayerDir);
        chrMvmnt.t_mesh.rotation = wantedRotation;
    }

    // void ManagerThrowControl()
    // {
        
    //     if(Input.GetMouseButton(1))
    //     {
            
    //         Vector3 mouseToPlayerDir = MouseUtils.MousePositonRelativeToCamera(mainCamera, chrMvmnt.t_mesh, terrainMask);
    //          Quaternion wantedRotation = Quaternion.LookRotation(mouseToPlayerDir);
    //         chrMvmnt.t_mesh.rotation = wantedRotation;
        
    //         if(Input.GetMouseButton(0))
    //         {
    //             playerTriggers.ContinueAimToThrow();
    //         }
    //         else if(Input.GetMouseButtonUp(0))
    //         {
    //             if(endurance.GetCurrentEndurance() >= character.energyConsumption.throwEnergy)
    //             {
    //                 endurance.ReduceEndurance(character.energyConsumption.throwEnergy);
    //                 AttackTrigger attackTrigger = attackTriggersManager.GetCurrentAttackTrigger();
    //                 attackTrigger.SetTriggerType(TriggerType.Distant);
    //                 attackTrigger.ReleaseAttack();
    //                 playerTriggers.ResetThrowState();
    //                 controllerMode = ControllerMode.DefaultMovement;
    //             }
    //             else
    //             {
    //                 playerTriggers.CancelThrow();
    //                 controllerMode = ControllerMode.DefaultMovement;
    //             }
    //         }
  
    //     }
    //     else 
    //     {
    //         playerTriggers.CancelThrow();
    //         controllerMode = ControllerMode.DefaultMovement;

    //     }
    //         //of weapon attack in weapon script, with some interface

    // }


    // void ManageSettingTrapControl()
    // {
    //     if(Input.GetKey(KeyCode.Escape))
    //     {
    //         controllerMode = ControllerMode.DefaultMovement;
    //         attachmentManager.ResetAttachmentProcess();
    //         attachmentManager.enabled = false;
    //         }
    //     else if(Input.GetMouseButtonDown(0))
    //     {
    //         Debug.Log("Object has been sticked!");

    //         bool finished = attachmentManager.onAttach?.Invoke() ?? false;
    //         if(finished)
    //             controllerMode = ControllerMode.DefaultMovement;
    //     }
    // }

    void OnInteraction()
    {
        if(obstacleInteractionManager.obstacleInteraction != null)
        {
            if (obstacleInteractionManager.GetCurrentAnimType() == ObstacleAnimType.vault)
                character.StartTriggerAction(obstacleInteractionManager.InteractObstacle, attackTriggersManager.StartAttack, interactionBonus.UntilVault);
            else
                character.StartTriggerAction(obstacleInteractionManager.InteractObstacle);
        }
    }

    // public void SwitchActionMap(ControllerMode mode) 
    // {
    //     // var actionMap = playerInput.actions.FindActionMap(mode.ToString());
    //     playerInput.currentActionMap.Disable();

    //     playerInput.SwitchCurrentActionMap(mode.ToString());
    //     playerInput.currentActionMap.Enable();
    //     // playerInput.SwitchCurrentActionMap(mode.ToString());
    //     Debug.Log($"CurrentActionMAp { playerInput.currentActionMap}");
    // }

    void OnInventoryEnter()
    {
        // SwitchActionMap(ControllerMode.Inventory);
        controls.Inventory.Enable();
        controls.DefaultMovement.Disable();
        inventoryUI.GoToInventory();
    }

    // void OnStartAiming()
    // {
    //     if(chrMvmnt.GetHoldMode() != WeaponType.None)
    //         {
    //             controls.Aiming.Enable();
    //             controls.DefaultMovement.Disable();
    //             // SwitchActionMap(ControllerMode.Aiming);
    //             playerTriggers.StartAim();

    //         }
    // }

    void OnReleaseThrow()
    {
        if(chrMvmnt.GetHoldMode() != WeaponType.None)
        {
            endurance.ReduceEndurance(character.energyConsumption.throwEnergy);
            AttackTrigger attackTrigger = attackTriggersManager.GetCurrentAttackTrigger();
            attackTrigger.SetTriggerType(TriggerType.Distant);
            character.StartTriggerAction(attackTriggersManager.StartAttack);
            // attackTrigger.ReleaseAttack();
            // yield return new WaitForSeconds(0.05f);
        }
        // controllerMode = ControllerMode.DefaultMovement;
    }

    // void OnStopAiming()
    // {
    //     playerTriggers.CancelThrow();
    //     controls.DefaultMovement.Enable();
    //     controls.Aiming.Disable();
    //     // playerInput.SwitchCurrentActionMap(ControllerMode.DefaultMovement.ToString());
    // }

    void OnPickupItem()
    {
        bool success = itemPickupManager.PickItem();
        if(success)
            StartCoroutine(playerAnimationController.PickupItem());
    }

    void OnAttack()
    {
        character.TryToRunFightMode();
        if(specialAttacks.isCandidateToDie)
        {
            character.StartTriggerAction(specialAttacks.KillWhenFaint);
        }
        else
        {
            AttackTrigger attackTrigger = attackTriggersManager.GetCurrentAttackTrigger();
            attackTrigger.SetTriggerType(TriggerType.Melee);
            Debug.Log($"CurrentTrigger {attackTriggersManager.GetDefaultAttackable()}");
            character.StartTriggerAction(attackTriggersManager.StartAttack);
        }
    }



    // void ManageNormalControl()
    // {
    //         // character.SetDefaultMovement();
    //         // if (Input.GetMouseButtonDown(0))
    //         // {
    //         //     // if(interactionBonus.isVaultContext)
    //         //     // {
    //         //     //     StartCoroutine(interactionBonus.SetToKickWhileVault());
    //         //     // }
    //         //     if(specialAttacks.isCandidateToDie)
    //         //     {
    //         //         character.StartTriggerAction(specialAttacks.KillWhenFaint);
    //         //     }
    //         //     else
    //         //     {
    //         //         AttackTrigger attackTrigger = attackTriggersManager.GetCurrentAttackTrigger();
    //         //         attackTrigger.SetTriggerType(TriggerType.Melee);
    //         //         Debug.Log($"CurrentTrigger {attackTriggersManager.GetDefaultAttackable()}");
    //         //         character.StartTriggerAction(attackTriggersManager.StartAttack);

    //                 // if(chrMvmnt.GetHoldMode() == WeaponType.WoddenStick)
    //                 // {
    //                 //     character.StartTriggerAction(playerTriggers.StickAttack);
    //                 // }
    //                 // else if(chrMvmnt.GetHoldMode() == WeaponType.Knife)
    //                 // {
    //                 //     character.StartTriggerAction(playerTriggers.KnifeAttack);
    //                 // }
    //                 // else
    //                 //     character.StartTriggerAction(playerTriggers.Kick);
    //             }

    //         }

            // else if(Input.GetKey(KeyCode.C))
            // {
            //     character.SetToCrounch();
            // }
            // else if(Input.GetKey(KeyCode.R))
            // {
            //     Debug.Log("Here drop object");
            // }
            // else if(Input.GetKeyDown(KeyCode.F))
            // {

            //     bool success = itemPickupManager.PickItem();
            //     if(success)
            //         StartCoroutine(playerAnimationController.PickupItem());
            // }   
            // else if(Input.GetKey(KeyCode.Tab))
            // {

            // }

    // }

    

    
}
