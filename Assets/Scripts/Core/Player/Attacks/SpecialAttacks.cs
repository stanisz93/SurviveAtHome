using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpecialAttacks : MonoBehaviour
{

    public Animator animator;
    public bool isCandidateToDie = false;
    public float playerDistanceFromHead = 0.2f;
    public StressReceiver stressReceiver;
    private PlayerTriggers playerTriggers;

    [SerializeField]
    private KillUtils target;
    private Transform playerMesh;
    private Transform player;
    [SerializeField]
    private bool isKillingEvent = false;
    // private OpponentMagnet opponentMagnet;


    // Start is called before the first frame update
    // void Start()
    // {
    //     opponentMagnet = GetComponent<OpponentMagnet>();

    // }

    // Update is called once per frame
    private void Start() {
        playerTriggers = GetComponentInParent<PlayerTriggers>();
        player = GetComponentInParent<Character>().transform;
    }
    public void SetTarget(Opponent opponent)
    {
        target = opponent.GetComponent<KillUtils>();
        isCandidateToDie = true;
    }

    public bool isTarget(Opponent opponent)
    {
        return opponent == target;
    }

    public void RemoveTarget()
    {
        if(!isKillingEvent)
            {
                target = null;
                isCandidateToDie = false;
            }
    }

    public void KillWhenFaint()
    {

            isKillingEvent = true;
            Transform opponentHead = target.GetHeadPos();
            Sequence seq = DOTween.Sequence();
            Vector3 dir = TransformUtils.GetXZDirectionWithMargin(transform, opponentHead, playerDistanceFromHead);
            playerTriggers.GetComponent<Collider>().enabled = false;
            seq.Join(player.DOMove(player.position + dir, 0.3f));
            Vector3 rotateDir = TransformUtils.GetXZDirection(transform, opponentHead);
            seq.Join(transform.DORotate(Quaternion.LookRotation(rotateDir).eulerAngles, 0.2f));
            seq.AppendCallback(() => playerTriggers.GetComponent<Collider>().enabled = true);
            playerTriggers.BlockMovement();
            playerTriggers.BlockTrigger();
            animator.SetTrigger("SquashHead");
        
    }

    public void InvokeOpponentKillReaction()
    {
        stressReceiver.InduceStress(2f);
        
        target.GotKilled();
        isKillingEvent = false;

        // HERE, disabling all scripts
        //and make it maybe a ragdoll?
    }

}
