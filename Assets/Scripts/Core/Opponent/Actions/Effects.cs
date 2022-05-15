using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleEffect {Blood, Push};
public class Effects : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject pfBloodEffect;
    public GameObject pfPushEffect;

    GameObject GetParticlePrefab(ParticleEffect effect)
    {
        GameObject pfEffect;
        switch(effect)
        {
            case ParticleEffect.Blood:
            {
                pfEffect = pfBloodEffect;
                break;
            }
            case ParticleEffect.Push:
            {
                pfEffect = pfPushEffect;
                break;
            }
            default:
            {
                pfEffect = null;
                break;
            }
        }
        return pfEffect;
    }

    public void RunParticleEffect(ParticleEffect pEffect, Vector3 pos)
    {
        GameObject effect = Instantiate(GetParticlePrefab(pEffect), pos, Quaternion.identity);
        var particleSystem = effect.GetComponent<ParticleSystem>();
        particleSystem.Play();
    }

    // Update is called once per frame
}
