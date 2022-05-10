using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerMode {Normal, Inventory, SettingTrap};

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterMovement))]
public class PlayerInput : MonoBehaviour
{

    public InventoryUI inventoryUI;
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

    private Transform characterMesh;
    private Camera mainCamera;
    


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
        characterMesh = GameObject.FindWithTag("PlayerMesh").transform;
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            
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
                character.AddMovementInput(0f, 0f, false);
            else
                character.AddMovementInput(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), true);
            
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
            // highlightedTile.TryToCraft();
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
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C))
            {
                if(playerTriggers.Slidable != null)
                    character.StartTriggerAction(playerTriggers.Slide);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if(chrMvmnt.GetHoldMode() == HoldMode.WoddenStick)
                {
                    character.StartTriggerAction(playerTriggers.StickAttack);
                }
                else
                    character.StartTriggerAction(playerTriggers.Kick);
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
