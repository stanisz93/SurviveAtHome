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

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        collider = GetComponent<CapsuleCollider>();
        chrMvmnt = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        character.AddMovementInput(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        // character.SetDefaultMovement();
        if (!character.isSliding && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C))
        {
            if (character.Slidable != null)
            {
                character.SetToSlide();
                character.Slide();
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
        else
        {
            character.SetToWalk();
        }
    }
    
}
