
using System.Collections;
using UnityEngine;

class LayerInvoker : MonoBehaviour
{
    public Animator animator;
    private float currentAnimWeight = 1f;


    public IEnumerator RunUntil(string animName, int layer, float animLifeTime)
    {
        animator.Play(animName, layer, 0f);
        yield return new WaitForSeconds(animLifeTime);
    }

    public IEnumerator SmoothlyFadeOutLayer(float fadeOutTime, int layer)
    {
    
        float timeElapsed = 0f;
        while(timeElapsed < fadeOutTime)
        {
            currentAnimWeight = Mathf.Lerp(1f, 0.0f, timeElapsed / fadeOutTime);
            timeElapsed += Time.deltaTime;
            animator.SetLayerWeight(1, currentAnimWeight);
            yield return null;
        }
    }

    public void Reset()
    {
        currentAnimWeight = 1f;
        animator.SetLayerWeight(1, 1f);

    }
}
