using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialKills : MonoBehaviour
{
    public Transform head;
    public List<GameObject> pfBloodTextures;
    public GameObject pfBloodKillEffect;

    public GameObject pfThrowKillEffect;
    public float bloodEmmiterHeight = 0.1f;
    public float bloodTextureActivateDelay = 0.2f;

    public float bloodTextureThrowDelay = 0.2f; // texture for throwing
    public float bloodTextureOffsetAlongSpine = 0f;
    private Animator animator;
    private VisionFieldOfView vfov;
    private Transform floor;
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

     IEnumerator InstantiateBloodTexture(float delay, Vector3 xzPos)
    {
        yield return new WaitForSeconds(delay);
        Vector3 pos = new Vector3(xzPos.x, floor.position.y + 0.002f, xzPos.z);
        float finalOffset = bloodTextureOffsetAlongSpine + Random.Range(-0.2f, 0.2f);
        pos = pos + bloodTextureOffsetAlongSpine * new Vector3(head.right.x, 0f, head.right.z);
        int randomTextureIdx = Random.Range(0, pfBloodTextures.Count - 1);
        Vector3 headFwdXZDirection = new Vector3(head.forward.x, 0, head.forward.z);
        Instantiate(pfBloodTextures[randomTextureIdx], pos, Quaternion.LookRotation(headFwdXZDirection));
       
    }
    // Update is called once per frame
    public void GotKilled()
    {
        vfov.TurnOffSense();
        Vector3 headRightXZDirection = new Vector3(head.right.x, 0, head.right.z);
        Vector3 pos = new Vector3(head.position.x, floor.position.y + bloodEmmiterHeight, head.position.z);
        Instantiate(pfBloodKillEffect, pos, Quaternion.LookRotation(-headRightXZDirection));
        StartCoroutine(InstantiateBloodTexture(bloodTextureActivateDelay, head.position));
        animator.SetTrigger("KickKill");
        GetComponent<Ragdoll>().ToggleRagdoll();
        Invoke("DestroyOpponentObject", 0.3f);

    }


    public void GotKilledByThrow(Transform throwTransform)
    {
        animator.SetTrigger("DieByThrow");
        GetComponent<Ragdoll>().ToggleRagdoll();
        GetComponentInChildren<TaskManager>().BlockAnyTaskAssigning();
        vfov.TurnOffSense();
        Instantiate(pfThrowKillEffect, throwTransform.position, Quaternion.identity, throwTransform);
        StartCoroutine(InstantiateBloodTexture(bloodTextureThrowDelay, throwTransform.position));
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        Invoke("DestroyOpponentObject", 1f);
    }

    void DestroyOpponentObject()
    {
        GetComponent<Ragdoll>().ToogleRigidbodies(false);

        var components = GetComponents<MonoBehaviour>();
        foreach( var t in components )
        {
            if (t.GetType().Name != "Ragdoll")
                Destroy(t);
        }
    }
}
