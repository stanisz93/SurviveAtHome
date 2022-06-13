using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEventController : MonoBehaviour
{

    // Start is called before the first frame update
    public float TrailKickDecay = 0.1f;
    private Animator animator;
    private PlayerInput playerInput;

    private TrailRenderer kickTrail;

  
    void Start()
    {
        kickTrail = GetComponentInChildren<TrailRenderer>();
        kickTrail.enabled = false;
    }

        
    IEnumerator TurnOffKickTrail()
    {
        yield return new WaitForSeconds(TrailKickDecay);
        kickTrail.enabled = false;
    }
    public void ToogleKickTrail()
    {
        kickTrail.enabled = true;
        StartCoroutine(TurnOffKickTrail());
    }


    public void ResetDefaultHoldPosition()
    {
        DefendItem stick = GetComponentInChildren<DefendItem>();
        if(stick != null)
        {
            stick.ChangeWeaponPositionToHold();
        }
    }





    // Update is called once per frame
}
