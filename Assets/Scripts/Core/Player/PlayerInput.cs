using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerMode {Normal, Inventory};

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

    private ControllerMode controllerMode;

    


    // Start is called before the first frame update
    void Start()
    {
        controllerMode = ControllerMode.Normal;
        character = GetComponent<Character>();
        collider = GetComponent<CapsuleCollider>();
        chrMvmnt = GetComponent<CharacterMovement>();
        playerTriggers = GetComponent<PlayerTriggers>();
        itemPickupManager = GetComponent<ItemPickupManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerTriggers.dying)
        {
            character.AddMovementInput(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
  
            if (controllerMode == ControllerMode.Normal)
            {
                ManageNormalControl();
            }
            else if(controllerMode == ControllerMode.Inventory)
            {   

                inventoryUI.ContainsAnyTile(Input.mousePosition);
                if(!Input.GetKey(KeyCode.Tab))
                {
                    
                    inventoryUI.LeftInventory();
                    controllerMode = ControllerMode.Normal;
                }
            }
        }
    }

    void ManageNormalControl()
    {
            // character.SetDefaultMovement();
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C))
            {
                if (playerTriggers.Slidable != null)
                {
                    character.triggeredAction = playerTriggers.Slide;
                }
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                character.SetToRun();
            }
            else if(Input.GetKey(KeyCode.C))
            {
                character.SetToCrounch();
            }
            else if(Input.GetKeyDown(KeyCode.F))
            {
                itemPickupManager.PickItem();
            }
            else if(Input.GetKey(KeyCode.Tab))
            {
                inventoryUI.GoToInventory();
                controllerMode = ControllerMode.Inventory;
            }
            else
            {
                character.SetToWalk();
            }
    }

    

    
}
