using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum BonusMode {SuperKick, Normal}
public class HitBonus : MonoBehaviour
{//hit every zombie hit unit got damaged
    public Gradient gradient;
    public Image bonusFillImg;
    public Image bonusBorderImg;
    public Image bonusPulseImg;
    public Text counterTxt;
    public Slider slider;
    public int hitsToBonus = 3;
    // Start is called before the first frame update
    public int hitCounts = 0;
    private int maxBonus = 50;

    private Vector3 initScale;
    private Tween pulseTween;
    

    private BonusMode bonusMode = BonusMode.Normal;
    private bool superHitMode = false;
    private InteractionBonus interactionBonus;

    private bool isBonusAnimationFinished = true;

    //sometimes some other triggers could temporarily turn on SuperKickmode
    //fi the trigger was temporar one, we want still to keep player bonus
    private bool isTemporalBoost = false;
    private BonusMode beforeTemporalBoost;


    void Start()
    {
        initScale = bonusFillImg.rectTransform.localScale;
        slider.maxValue = hitsToBonus;
        bonusFillImg.enabled = false;
        bonusPulseImg.enabled = false; 
        bonusBorderImg.enabled = false;
        counterTxt.enabled = false;
        interactionBonus = GetComponentInChildren<InteractionBonus>();
    }

    void UpdateBonus()
    {
        if(hitCounts % hitsToBonus  == 0 && hitCounts != 0 && !superHitMode)
        {
            // interactionBonus.TurnOnBonusInteraction();
            bonusMode = BonusMode.SuperKick;
            bonusPulseImg.enabled = true;
            var tempColor = bonusPulseImg.color;
            tempColor.a = 1f;
            bonusPulseImg.color = tempColor;
            bonusPulseImg.DOKill();
            pulseTween = bonusPulseImg.DOFade(0f, 0.3f).SetLoops(-1, LoopType.Yoyo);
            superHitMode = true;
        }
        else
        {
            bonusPulseImg.enabled = false;
            superHitMode = false;
            bonusMode = BonusMode.Normal;
        }
    }

    public void SetSuperKick()
    {
        isTemporalBoost = true;
        beforeTemporalBoost = bonusMode;
        bonusMode = BonusMode.SuperKick;
    }

    public void RemoveSuperKick()
    {
        if(isTemporalBoost)
            {
                bonusMode = beforeTemporalBoost;
                beforeTemporalBoost = BonusMode.Normal;
                isTemporalBoost = false;
            }
        else
            bonusMode = BonusMode.Normal;
    }

    public BonusMode GetBonusMode()
    {
        return bonusMode;
    }

    // Update is called once per frame
    public void IncreaseCounts(AttackTrigger attackTrigger)
    {
        if(hitCounts != 0 && superHitMode)
            slider.value = 0;
        else
        {
            hitCounts += 1;
            slider.value = (hitCounts-1) % hitsToBonus + 1;
        }    

        if(!bonusFillImg.enabled && hitCounts > 0)
        {
            bonusFillImg.enabled = true;
            bonusBorderImg.enabled = true;
            counterTxt.enabled = true;

        }

        counterTxt.text = $"X{hitCounts.ToString()}";
        RunBonusAnimation();
        EvaluateColor();
        UpdateBonus();
    }

    void EvaluateColor()
    {
        float frac = (float) hitCounts / maxBonus;
        var color = gradient.Evaluate(frac);
        counterTxt.color = color;
        bonusFillImg.color = color;
        bonusPulseImg.color = color;
    }

    void RunBonusAnimation()
    {
        if(isBonusAnimationFinished)
        {
            isBonusAnimationFinished = false;
            Sequence s = DOTween.Sequence();
            s.Append(bonusFillImg.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f).SetLoops(2, LoopType.Yoyo));
            s.Join(bonusBorderImg.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f).SetLoops(2, LoopType.Yoyo));
            s.AppendCallback(() => isBonusAnimationFinished = true);
        }
    }

    void  OnDisableImage() {
        bonusFillImg.enabled = false;
        bonusBorderImg.enabled = false;
        counterTxt.enabled = false;
        if(pulseTween.IsActive())
            DOTween.Kill(pulseTween);
        bonusPulseImg.enabled = false;
        bonusBorderImg.rectTransform.localScale = initScale;
        bonusFillImg.rectTransform.localScale = initScale;
    }

    public void FadeOutImage()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(bonusFillImg.DOFade(0f, 0.1f));
        
        sequence.AppendCallback(()=>OnDisableImage());
    }

    public void ResetCounts()
    {
        FadeOutImage();
        hitCounts = 0;
        UpdateBonus();
        // bonusImg.enabled = false;
    }


}
