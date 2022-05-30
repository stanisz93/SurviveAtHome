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
                character.StartTriggerAction(playerTriggers.ThrowWeapon); //I should create each logic
                playerTriggers.ResetThrowState();
                controllerMode = ControllerMode.Normal;
            }
  
        }
        else 
        {
            playerTriggers.RelaseThrow();
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
                    character.StartTriggerAction(obstacleInteractionManager.InteractObstacle);
            }
            else if(Input.GetMouseButtonDown(1) && chrMvmnt.GetHoldMode() != HoldMode.Default)
            {
                controllerMode = ControllerMode.Throwing;
                playerTriggers.StartAim();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if(interactionBonus.isVaultContext)
                {
                    StartCoroutine(interactionBonus.SetToKickWhileVault());
                }
                else if(specialAttacks.isCandidateToDie)
                {
                    character.StartTriggerAction(specialAttacks.KillWhenFaint);
                }
                else
                {
                    if(chrMvmnt.GetHoldMode() == HoldMode.WoddenStick)
                    {
                        character.StartTriggerAction(playerTriggers.StickAttack);
                    }
                    else if(chrMvmnt.GetHoldMode() == HoldMode.Knife)
                    {
                        character.StartTriggerAction(playerTriggers.KnifeAttack);
                    }
                    else
                        character.StartTriggerAction(playerTriggers.Kick);
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

                Action collectProcess = itemPickupManager.PickItem();
                if(collectProcess != null)
                    StartCoroutine(playerAnimationController.PickupItem(collectProcess));
            }
            else if(Input.GetKey(KeyCode.Tab))
            {
                inventoryUI.GoToInventory();
                controllerMode = ControllerMode.Inventory;
            }

    }

    

    
}
