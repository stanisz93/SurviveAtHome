using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillUtils : MonoBehaviour
{
    public Transform head;
    public List<GameObject> pfBloodTextures;
    public GameObject pfBloodKillEffect;

    public GameObject pfThrowKillEffect;
    public float bloodEmmiterHeight = 0.1f;
    public float bloodTextureActivateDelay = 0.2f;

    public float offsetAlongOpponentSpine = 0.0f;

    public float bloodTextureThrowDelay = 0.2f; // texture for throwing
    private Animator animator;
    private VisionFieldOfView vfov;
    private Transform floor;

    bool stoppedRagdoll = false;

    // Start is called before the first frame update
    void Start()
    {
        floor = GameObject.FindWithTag("Floor").transform;
        vfov = GetComponentInChildren<VisionFieldOfView>();
        animator = GetComponentInChildren<Animator>();
    }

    public Transform GetHeadPos()
    {
        return head;
    }

     public IEnumerator InstantiateBloodTexture(float delay, Transform xzPos, float offsetAlongOpponentSpine)
    {
        while(!stoppedRagdoll)
        {
            yield return new WaitForSeconds(delay);
            Vector3 pos = new Vector3(xzPos.position.x, floor.position.y + 0.002f, xzPos.position.z);
            float finalOffset = offsetAlongOpponentSpine + Random.Range(-0.2f, 0.2f);
            pos = pos + offsetAlongOpponentSpine * new Vector3(head.right.x, 0f, head.right.z);
            int randomTextureIdx = Random.Range(0, pfBloodTextures.Count - 1);
            Vector3 headFwdXZDirection = new Vector3(head.forward.x, 0, head.forward.z);
            Instantiate(pfBloodTextures[randomTextureIdx], pos, Quaternion.LookRotation(headFwdXZDirection));
        }
    }
    // Update is called once per frame
    public void GotKilled()
    {
        Vector3 headRightXZDirection = new Vector3(head.right.x, 0, head.right.z);
        Vector3 pos = new Vector3(head.position.x, floor.position.y + bloodEmmiterHeight, head.position.z);
        Instantiate(pfBloodKillEffect, pos, Quaternion.LookRotation(-headRightXZDirection));
        StartCoroutine(InstantiateBloodTexture(bloodTextureActivateDelay, head, offsetAlongOpponentSpine));
        animator.SetTrigger("KickKill");
        GetComponent<Ragdoll>().ToggleRagdoll(Vector3.zero);
        StartCoroutine(DestroyOpponentObject(0.3f));

    }


    public IEnumerator DestroyOpponentObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Ragdoll>().ToogleRigidbodies(false);
        stoppedRagdoll = true;

        var components = GetComponents<MonoBehaviour>();
        foreach( var t in components )
        {
            if (t.GetType().Name != "Ragdoll")
                Destroy(t);
        }
    }
}
