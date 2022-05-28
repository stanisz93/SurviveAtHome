using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialKills : MonoBehaviour
{
    public Transform head;
    public Transform floor;
    public List<GameObject> pfBloodTextures;
    public GameObject pfBloodKillEffect;
    public float bloodEmmiterHeight = 0.1f;
    public float bloodTextureActivateDelay = 0.2f;
    public float bloodTextureOffsetAlongSpine = 0f;
    private Animator animator;
    private VisionFieldOfView vfov;
    // Start is called before the first frame update
    void Start()
    {
        vfov = GetComponentInChildren<VisionFieldOfView>();
        animator = GetComponentInChildren<Animator>();
    }

    public Transform GetHeadPos()
    {
        return head;
    }

    void InstantiateBloodTexture()
    {
        Vector3 pos = new Vector3(head.position.x, floor.position.y + 0.002f, head.position.z);
        Vector3 headFwdXZDirection = new Vector3(head.forward.x, 0, head.forward.z);
        float finalOffset = bloodTextureOffsetAlongSpine + Random.Range(-0.2f, 0.2f);
        pos = pos + bloodTextureOffsetAlongSpine * new Vector3(head.right.x, 0f, head.right.z);
        int randomTextureIdx = Random.Range(0, pfBloodTextures.Count - 1);
        Instantiate(pfBloodTextures[randomTextureIdx], pos, Quaternion.LookRotation(headFwdXZDirection));
       
    }
    // Update is called once per frame
    public void GotKilled()
    {
        vfov.TurnOffSense();
         Vector3 headRightXZDirection = new Vector3(head.right.x, 0, head.right.z);
        Vector3 pos = new Vector3(head.position.x, floor.position.y + bloodEmmiterHeight, head.position.z);
        Instantiate(pfBloodKillEffect, pos, Quaternion.LookRotation(-headRightXZDirection));
        Invoke("InstantiateBloodTexture", bloodTextureActivateDelay);
        animator.SetTrigger("KickKill");
        Invoke("DestroyOpponentObject", 2f);
    }
    void DestroyOpponentObject()
    {
        var components = GetComponents<MonoBehaviour>();
        foreach( var t in components )
        {
            Destroy(t);
        }
    }
}
