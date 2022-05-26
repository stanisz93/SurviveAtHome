using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentEventController : MonoBehaviour
{
    // Start is called before the first frame update
    

    public OpponentHit opponentHit;
    private Character character;
    private Opponent opponent;
    private TaskManager taskManager;
    private OpponentActions opponentActions;


    void Start()
    {
        opponent = gameObject.GetComponentInParent<Opponent>();
        character = GameObject.FindWithTag("Player").GetComponent<Character>();
        opponentActions = GetComponentInParent<OpponentActions>();
    }

    // Update is called once per frame

    public void EnableHitMoment()
    {
        opponentHit.enabled = true;

    }
    public void DisableHitMoment()
    {
        opponentHit.enabled = false;
    }

    public void WakeUp()
    {
        opponentActions.SetOpponentMode(OpponentMode.Exploring);
    }



}
