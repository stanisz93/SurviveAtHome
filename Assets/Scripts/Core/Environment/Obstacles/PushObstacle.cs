using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObstacle : MonoBehaviour, IObstacleInteractable
{
    // Start is called before the first frame update
    
    public ObstacleAnimType animType{ get {return ObstacleAnimType.PushDoor;}}

    public float forceMultiplier = 100f;
    
    public float forceEffectTimeDecay = 0.1f;

    public float timeOfPushing = 1f;
    
    public float pushWhenHit = 3f;
    public TrailRenderer tr;

    private bool isForceApplied = false;

    private void Start() {
        tr = GetComponentInChildren<TrailRenderer>();
    }
    public void PerformAction(Animator animator, Character character)
    {
        animator.SetTrigger("PushDoor");
        ApplyForce(character.GetForward());

    }

        public void ApplyForce(Vector3 force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            isForceApplied = true;
            rb.AddForce(force * forceMultiplier, ForceMode.Impulse);
            tr.enabled = true;
            StartCoroutine(ForceEffect());
        }
    }

    IEnumerator ForceEffect()
    {
        yield return new WaitForSeconds(forceEffectTimeDecay);
        isForceApplied = false;
        tr.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        
        Opponent opponent = other.GetComponent<Opponent>();
        if(opponent != null && isForceApplied)
        {
            opponent.GetComponent<MeleeReaction>().InvokeReaction(DamageType.ToTheGround, WeaponType.None, transform, pushWhenHit, timeOfPushing);
            //opponent.GotPushed();
        } 
    }
}
