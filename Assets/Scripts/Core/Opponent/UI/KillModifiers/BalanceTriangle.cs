using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public enum TurnOffReason {TooFar, Refreshed, Knocked};
public class BalanceTriangle : MonoBehaviour
{

    [SerializeField]
    private Image arrowImage;
    [SerializeField]
    private float arrowSpeed = 0.1f;
    [SerializeField]
    private Image fillBonusImg;
    [SerializeField]
    private Image leverageImg;
    [SerializeField]
    private AnimationCurve levarageRotationControl;

    [SerializeField]
    private Image triangleFillImg;
    [SerializeField]
    private AnimationCurve fillTriangleCurve;
    [SerializeField]
    private float durationPerEachUnit = 0.005f; //Time per each Unit of localPosition from 0 to 44(-44)

    [SerializeField]
    private Image triangleBorder;

/// PARAMETERS THAT AFFECT HOW DIFFICULT IS ENEMY
    [SerializeField]
    private float triangleFillLevel = 5; // value that tell how many 
    //full round (reach exact yellow circle) need to be applied to fill triangle
    //if level = 5 it means that you need to apply 5 times 0.5 value 
    [SerializeField]
    private float timeToDeactivate = 2f;
    [SerializeField]
    private float leverageRotationSpeed = 0.05f;
    [SerializeField]
    private float decreaseTriangleSpeed = 0.1f;



    [SerializeField]
    private float fillBonusDuration = 0.05f;
    [SerializeField]
    private ParticleSystem particleEffect;
    [SerializeField]
    private Text breakText;


    private CanvasGroup group;
    private RectTransform arrowRectTransform;
    private RectTransform particleRectTransform;
    private PlayerInput playerInput;
    int currentDirection;
    int currentSide;
    float currentArrowPosition;



    float arrowMin = -44f;
    float arrowMax = 44f;
    PlayerActions controls;

    bool isScoreCalculating = false;
    bool isActive = false;
    bool isPulseStarted = false;
    Coroutine calculateScoreCoroutine;

    Coroutine triangleRegenerateCoroutine;

    Sequence lowTrianglePulseSequence;
    float defaultPlayerPower = 1f;
    float triangleBreakValue = 0.8f; //Above that fill, opponent is knocked out
    // Start is called before the first frame update
    void Start()
    {
        breakText.enabled = false;
        group = GetComponent<CanvasGroup>();
        particleRectTransform = particleEffect.GetComponent<RectTransform>();
        playerInput = GetComponent<PlayerInput>();
        arrowRectTransform = arrowImage.GetComponent<RectTransform>();
        SetRandomTrianglePosition();
        controls = new PlayerActions();
        group.alpha = 0f;
        RestartTriangle();
        //below part should be matched to each opponent separately
        controls.DefaultMovement.Attack.performed += ctx => OnAttack();
        
    }

    void RestartTriangle()
    {
        isPulseStarted = false;
        triangleFillImg.fillAmount = 0.0f;
        fillBonusImg.fillAmount = 0;
        leverageImg.rectTransform.localScale = new Vector3(0f, 0f, 1f);
        triangleFillImg.rectTransform.localScale = new Vector3(0f, 0f, 1f);
        leverageImg.rectTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
        
    }

    void SetRandomTrianglePosition()
    {
        var shape = particleEffect.shape;
        shape.rotation = new Vector3(-90f, 90f, 90f);
        currentArrowPosition = Random.Range(arrowMin, arrowMax);
        arrowImage.DOFade(1f, 0.1f);
        currentDirection = (Random.value >= 0.5) ? 1 : -1;
    }

    IEnumerator OpponentRegenerate()
    {
        var rect = leverageImg.rectTransform;
        while(Quaternion.Angle(rect.rotation, Quaternion.identity) > 0.01f) //rect.eulerAngles.z < 360f)
        {
            rect.eulerAngles = new Vector3(0f, 0f, rect.eulerAngles.z + leverageRotationSpeed);
            yield return null;
        }
        rect.localEulerAngles = new Vector3(0f, 0f, 0f);
        while(triangleFillImg.fillAmount > 0f)
        {
            triangleFillImg.fillAmount -= decreaseTriangleSpeed * Time.deltaTime;
            yield return null;

            if(triangleFillImg.fillAmount < 0.2f && !isPulseStarted)
            {
                Sequence seq = DOTween.Sequence().
                Append(triangleBorder.DOFade(0f, 0.1f)).
                Join(triangleFillImg.DOFade(0f, 0.1f)).
                SetLoops(-1, LoopType.Yoyo).
                OnKill(() => {
                    ColorUtils.SetImageAlphaColor(triangleBorder, 1f);
                    ColorUtils.SetImageAlphaColor(triangleFillImg, 1f);
                });
                lowTrianglePulseSequence = seq;
                isPulseStarted = true;
            }
            else if(isPulseStarted && triangleFillImg.fillAmount >= 0.2f)
            {
                if(lowTrianglePulseSequence.IsActive())
                    {
                        lowTrianglePulseSequence.Kill();
                        isPulseStarted = false;
                    }
            }
            yield return null;
        }
        TurnOffBonusDOTweenSequence(TurnOffReason.Refreshed);
    }


    Sequence TurnOffBonusDOTweenSequence(TurnOffReason turnReason)
    {   
        breakText.text = turnReason.ToString();
        breakText.color = new Color(breakText.color.r, breakText.color.g, breakText.color.b, 1f);
        Sequence s = DOTween.Sequence()
            .Append(group.DOFade(0.0f, 0.1f))
            .AppendCallback(() => breakText.enabled = true)
            .Append(breakText.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.1f))
            .Append(breakText.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f))
            .Append(breakText.transform.DOShakePosition(0.2f, new Vector3(4, 4, 0f), 30, 0))
            .AppendInterval(0.5f)
            .Append(breakText.DOFade(0f, 0.1f))
            .AppendCallback(() => isActive = false)
            .AppendCallback(RestartTriangle)
            .AppendCallback(() => {
                    if(lowTrianglePulseSequence.IsActive())
                        lowTrianglePulseSequence.Kill();
                    });
        return s;
    }

    IEnumerator ProcessTriangleFill(float uiScore, float playerWeaponPower)
    {

        float transfScore = (uiScore * 2 * playerWeaponPower) / (1f /triangleBreakValue * triangleFillLevel); //doubled because score max be at most 0.5;
        float finalScore = Mathf.Min(triangleBreakValue, triangleFillImg.fillAmount + transfScore);

        if(finalScore >= triangleBreakValue)
            {
                Sequence s = TurnOffBonusDOTweenSequence(TurnOffReason.Knocked);
                yield return s.WaitForCompletion();
                yield break;
            }

        Sequence fillTriangleSequence = DOTween.Sequence();
        particleRectTransform.localPosition = new Vector3(currentArrowPosition, particleRectTransform.localPosition.y, particleRectTransform.localPosition.z);
        var shape = particleEffect.shape;
        particleEffect.Play();

        float finalFillDuration = CalculateFinalFillDuration();
        fillTriangleSequence.Append(fillBonusImg.DOFillAmount(0.0f, finalFillDuration));
        // fillTriangleSequence.Join(triangleFillImg.transform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.1f).SetLoops(2, LoopType.Yoyo));
        fillTriangleSequence.Join(triangleFillImg.transform.DOShakePosition(finalFillDuration, new Vector3(5, 0, 0f), 30, 0));
        
        fillTriangleSequence.Join(particleRectTransform.DOLocalMoveX(currentSide * arrowMax, finalFillDuration));
        Vector3 finalRotation = new Vector3(CalulateParticleEmitterRotation(), particleEffect.shape.rotation.y, particleEffect.shape.rotation.z); 
        fillTriangleSequence.Join(DOTween.To(() => shape.rotation, x => shape.rotation = x, finalRotation, finalFillDuration));

        fillTriangleSequence.Join(triangleFillImg.DOFillAmount(finalScore, finalFillDuration).SetEase(fillTriangleCurve));
        fillTriangleSequence.AppendCallback(() => particleEffect.Stop());

        yield return fillTriangleSequence.WaitForCompletion();
    }

    float CaclulateLocalPositionToScore()
    {
        return currentSide * (1.0f / (arrowMin - arrowMax)) * currentArrowPosition + 0.5f;
    }

    float CalulateParticleEmitterRotation() //based on current position should be from -130, to 90
    {
        float startingAngle = -90f; //angle when pointer is in the middle
        float maxOffset = 50f;
        //let's say paritcleSystem is at right,
        //we want it to be -90 + 1 * 44 * (50 / 44) = -90 - 40 = -130
        //opposite -90 + 40 = -50
        return startingAngle + currentSide * arrowMax * maxOffset / arrowMax;
    }

    float CalculateFinalFillDuration()
    {
        return durationPerEachUnit * (arrowMax - Mathf.Abs(currentArrowPosition) + 3 * durationPerEachUnit);
    }

    IEnumerator CalculateScore(float playerWeaponPower)
    {
        if(triangleRegenerateCoroutine != null)
            {
                StopCoroutine(triangleRegenerateCoroutine);
            }
        fillBonusImg.fillOrigin = currentArrowPosition >= 0 ? (int) Image.OriginHorizontal.Right : (int) Image.OriginHorizontal.Left;
        isScoreCalculating = true;
        yield return null;
        
        Sequence s = DOTween.Sequence();
        // according to formula fillAmount = a*currentArrowPosition + b, where f(44) = a * 44 + b = 0 , and f(0) = b = 0.5 for Right 
        //above is for right 
        s.Append(arrowImage.transform.DOScale(new Vector3(2f, 2f, 1f), 0.1f).SetLoops(2, LoopType.Yoyo));
        float uiScore = CaclulateLocalPositionToScore();
        s.Join(fillBonusImg.DOFillAmount(uiScore, fillBonusDuration));
        s.Append(arrowImage.DOFade(0f, 0.1f));
        s.Join(leverageImg.rectTransform.DOLocalRotate(new Vector3(0f, 0f, -25f), 0.1f).SetEase(levarageRotationControl));
        yield return s.WaitForCompletion();

        // ProcessTriangleFill(currentScore, fillBonusSequence);
        yield return ProcessTriangleFill(uiScore, playerWeaponPower);

        if(isActive)
        {
            triangleRegenerateCoroutine = StartCoroutine(OpponentRegenerate());
        //Next turn
            SetRandomTrianglePosition();
            isScoreCalculating = false;
        }
    }


     void OnAttack()
     {
        if(!isActive)
        {
            isScoreCalculating = false;
            ColorUtils.SetImageAlphaColor(arrowImage, 1f);
            group.alpha = 1f;
            var sequence = DOTween.Sequence()
            .Append(leverageImg.rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1f), 0.1f))
            .Join(triangleFillImg.rectTransform.DOScale(new Vector3(2f, 2f, 1f), 0.1f))
            .Append(leverageImg.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.05f))
            .Join(triangleFillImg.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

            isActive = true;
            return;
        }
        if(!isScoreCalculating) // this should be only allowed if CalculationScore is finished
        {
            if(calculateScoreCoroutine != null)
                StopCoroutine(calculateScoreCoroutine);
            calculateScoreCoroutine = StartCoroutine(CalculateScore(defaultPlayerPower));
        }
        
     }

    // Update is called once per frame
    void Update()
    {
        if(!isScoreCalculating)
        {
            currentSide = currentArrowPosition >= 0 ? 1 : -1;
            if(currentArrowPosition < arrowMax || currentArrowPosition > arrowMin)
            {
                if(currentArrowPosition < arrowMin)
                    {
                        currentArrowPosition = arrowMin;
                        currentDirection = 1;
                    }
                else if(currentArrowPosition >= arrowMax)
                    {
                        currentArrowPosition = arrowMax;
                        currentDirection = -1;
                    }
            }
            arrowRectTransform.localPosition = new Vector3(currentArrowPosition, arrowRectTransform.localPosition.y, arrowRectTransform.localPosition.z);
            currentArrowPosition += currentDirection * arrowSpeed;
        }
    }
}
