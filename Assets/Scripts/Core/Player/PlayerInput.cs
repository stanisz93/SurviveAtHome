using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ControllerMode {Normal, Inventory, SettingTrap};

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterMovement))]
public class PlayerInput : MonoBehaviour
{

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
    public bool isVaultContext;
    private PlayerAnimatorEventController playerAnimatorEventController;

    // Start is called before the first frame update
    void Start()
    {
        controllerMode = ControllerMode.Normal;
        character = GetComponent<Character>();
        collider = GetComponent<CapsuleCollider>();
        chrMvmnt = GetComponent<CharacterMovement>();
        playerTriggers = GetComponent<PlayerTriggers>();
        playerAnimationController = GetComponent<PlayerAnimationController>(); 
        itemPickupManager = GetComponent<ItemPickupManager>();
        attachmentManager = GetComponent<AttachmentManager>();
        attachmentManager.enabled = false;
        inventoryUI =  GetComponentInChildren<InventoryUI>();
        characterMesh = GameObject.FindWithTag("PlayerMesh").transform;
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        hitBonus = GetComponent<HitBonus>();
        playerAnimatorEventController = GetComponentInChildren<PlayerAnimatorEventController>();

            
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
                if(playerTriggers.obstacleInteraction != null)
                    character.StartTriggerAction(playerTriggers.InteractObstacle);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if(isVaultContext)
                {
                    StartCoroutine(playerAnimatorEventController.SetToKickWhileVault());
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
