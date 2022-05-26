using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialKills : MonoBehaviour
{
    public Transform Head;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public Transform GetHeadPos()
    {
        return Head;
    }

    // Update is called once per frame
    public void GotKilled()
    {
        animator.SetTrigger("KickKill");
    }
}
