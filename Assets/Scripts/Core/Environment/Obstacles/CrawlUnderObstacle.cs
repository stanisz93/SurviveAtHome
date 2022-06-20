using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrawlUnderObstacle : MonoBehaviour, IObstacleInteractable
{
    // Start is called before the first frame update
    
    public GameObject pfSwooshEffect;
    public Transform particlePos;

    private Vector3 initalPos;
    public ObstacleAnimType animType{ get {return ObstacleAnimType.SlideUnderBed;}}
    bool isOccupied = false;
    private void Start() {
        initalPos = particlePos.position;
    }

    public void PerformAction(Animator animator, Character character)
    {
        particlePos.position = initalPos;
        animator.SetTrigger("GoUnderBed");
        character.SetControlMode(ControllerMode.UnderBed);
        character.ResetVelocity();
        character.ToogleKinematic();
        var swooshEffect = Instantiate(pfSwooshEffect, particlePos.position, Quaternion.identity);
        swooshEffect.transform.parent = particlePos;
        Vector3 towardBedDir = (transform.position - particlePos.position);
        towardBedDir = new Vector3(towardBedDir.x, 0f, towardBedDir.z).normalized;
        particlePos.DOMove(particlePos.position + towardBedDir*2f, 1f);
        swooshEffect.GetComponent<ParticleSystem>().Play();
        Destroy(swooshEffect, 1.0f);
    }

}
