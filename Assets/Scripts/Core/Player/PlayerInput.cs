using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ControllerMode {Normal, Inventory, SettingTrap, Throwing};

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterMovement))]
public class PlayerInput : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        controllerMode = ControllerMode.Normal;
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

    
    // Update is called once per frame
    void Update()
    {
        if (!playerTriggers.dying)
        {
            if(blockMovement)
                character.AddMovementInput(0f, 0f);
            else
                character.AddMovementInput(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                character.SetToRun();
            }
            else
            {
                character.SetToWalk();
            }

            if (controllerMode == ControllerMode.Normal)
            {
                ManageNormalControl();
            }
            else if(controllerMode == ControllerMode.Inventory)
            {   
                ManageInventory();

            }
            else if (controllerMode == ControllerMode.SettingTrap)
            {
                ManageSettingTrapControl();
            }
            else if(controllerMode == ControllerMode.Throwing)
            {
                ManagerThrowControl();
            }
        }
    }


    void ManageInventory()
    {
        var anyTile = inventoryUI.ContainsAnyTile(Input.mousePosition);
        if(anyTile is ItemTile)
            highlightedTile = (ItemTile)anyTile;
        if(Input.GetMouseButtonDown(0) && highlightedTile != null && !craftProcess)
        {
            startClick = Time.time;
            mouseIsClicked = true;
            StartCoroutine(CraftProcess(highlightedTile));
        }
        if(Input.GetMouseButtonUp(0))
        {
            mouseIsClicked = false;
            if(!craftProcess)
            {
                Debug.Log("Use item method!");
                inventoryUI.LeftInventory();
                controllerMode = ControllerMode.SettingTrap;
                attachmentManager.enabled = true;
            }
        }
        if(!Input.GetKey(KeyCode.Tab))
        {
            
            inventoryUI.LeftInventory();
            controllerMode = ControllerMode.Normal;
        }
    }

    void ManagerThrowControl()
    {
        
        if(Input.GetMouseButton(1))
        {
            
            Vector3 mouseToPlayerDir = MouseUtils.MousePositon(mainCamera, chrMvmnt.t_mesh, terrainMask);
             Quaternion wantedRotation = Quaternion.LookRotation(mouseToPlayerDir);
            chrMvmnt.t_mesh.rotation = wantedRotation;
        
            if(Input.GetMouseButton(0))
            {
                playerTriggers.ContinueAimToThrow();
            }
            else if(Input.GetMouseButtonUp(0))
            {
                AttackTrigger attackTrigger = attackTriggersManager.GetCurrentAttackTrigger();
                attackTrigger.SetTriggerType(TriggerType.Distant);
                attackTrigger.ReleaseAttack();
                playerTriggers.ResetThrowState();
                controllerMode = ControllerMode.Normal;
            }
  
        }
        else 
        {
            playerTriggers.CancelThrow();
            controllerMode = ControllerMode.Normal;

        }
            //of weapon attack in weapon script, with some interface

    }

    void ManageSettingTrapControl()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            controllerMode = ControllerMode.Normal;
            attachmentManager.ResetAttachmentProcess();
            attachmentManager.enabled = false;
            }
        else if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Object has been sticked!");

            bool finished = attachmentManager.onAttach?.Invoke() ?? false;
            if(finished)
                controllerMode = ControllerMode.Normal;
        }
    }

    void ManageNormalControl()
    {
            // character.SetDefaultMovement();
            if (Input.GetKey(KeyCode.V))
            {
                if(obstacleInteractionManager.obstacleInteraction != null)
                {
                    if (obstacleInteractionManager.obstacleInteraction.animType == ObstacleAnimType.vault)
                        character.StartTriggerAction(obstacleInteractionManager.InteractObstacle, attackTriggersManager.StartAttack, interactionBonus.UntilVault);
                    else
                        character.StartTriggerAction(obstacleInteractionManager.InteractObstacle);
                }
            }
            else if(Input.GetMouseButtonDown(1) && chrMvmnt.GetHoldMode() != WeaponType.None)
            {
                controllerMode = ControllerMode.Throwing;
                playerTriggers.StartAim();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                // if(interactionBonus.isVaultContext)
                // {
                //     StartCoroutine(interactionBonus.SetToKickWhileVault());
                // }
                if(specialAttacks.isCandidateToDie)
                {
                    character.StartTriggerAction(specialAttacks.KillWhenFaint);
                }
                else
                {
                    AttackTrigger attackTrigger = attackTriggersManager.GetCurrentAttackTrigger();
                    attackTrigger.SetTriggerType(TriggerType.Melee);
                    Debug.Log($"CurrentTrigger {attackTriggersManager.GetCurrentAttackable()}");
                    character.StartTriggerAction(attackTriggersManager.StartAttack);

                    // if(chrMvmnt.GetHoldMode() == WeaponType.WoddenStick)
                    // {
                    //     character.StartTriggerAction(playerTriggers.StickAttack);
                    // }
                    // else if(chrMvmnt.GetHoldMode() == WeaponType.Knife)
                    // {
                    //     character.StartTriggerAction(playerTriggers.KnifeAttack);
                    // }
                    // else
                    //     character.StartTriggerAction(playerTriggers.Kick);
                }

            }

            else if(Input.GetKey(KeyCode.C))
            {
                character.SetToCrounch();
            }
            else if(Input.GetKey(KeyCode.R))
            {
                Debug.Log("Here drop object");
            }
            else if(Input.GetKeyDown(KeyCode.F))
            {

                bool success = itemPickupManager.PickItem();
                if(success)
                    StartCoroutine(playerAnimationController.PickupItem());
            }   
            else if(Input.GetKey(KeyCode.Tab))
            {
                inventoryUI.GoToInventory();
                controllerMode = ControllerMode.Inventory;
            }

    }

    

    
}
