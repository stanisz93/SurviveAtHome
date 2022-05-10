using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentEventController : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Collider hitCollider;
    [SerializeField]
    private bool blockedTask;
    
    private Character character;
    private Opponent opponent;

    void Start()
    {
        opponent = gameObject.GetComponentInParent<Opponent>();
        character = GameObject.FindWithTag("Player").GetComponent<Character>();
        blockedTask = false;
        hitCollider.enabled = false;
    }

    // Update is called once per frame

    public void EnableHitMoment()
    {
        hitCollider.enabled = true;
    }
    public void DisableHitMoment()
    {
        hitCollider.enabled = false;
    }


}
