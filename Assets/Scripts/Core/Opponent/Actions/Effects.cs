using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    // Start is called before the first frame update

    public void RunParticleEffect(GameObject pf, Vector3 pos)
    {
        GameObject effect = Instantiate(pf, pos, Quaternion.identity);
        var particleSystem = effect.GetComponent<ParticleSystem>();
        particleSystem.Play();
    }

    // Update is called once per frame
}
