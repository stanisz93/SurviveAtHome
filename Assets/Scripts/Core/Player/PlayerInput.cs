using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CharacterMovement))]
public class PlayerInput : MonoBehaviour
{
    private Character character;
    private CharacterMovement chrMvmnt;
    private CapsuleCollider collider;

    private PlayerTriggers playerTriggers;
    private ItemPickupManager itemPickupManager;

    // Start is called before the first frame update
    void Start()
    {
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
            else
            {
                character.SetToWalk();
            }
        }
    }

    

    
}
