    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using DG.Tweening;
    class DoTweenUtils
    {
    
        static public void PoopUpImage(Image img, float scaleXY=1.5f, float scaleZ=1f)
        {
            img.transform.DOScale(new Vector3(scaleXY, scaleXY, scaleZ), 0.1f).SetLoops(2, LoopType.Yoyo);
        }

        static public void PoopUpImageFade(Image img, float scaleXY=1.5f, float scaleZ=1f, float fadeTime=0.1f)
        {   
            Sequence sequence = DOTween.Sequence();
            sequence.Append(img.transform.DOScale(new Vector3(scaleXY, scaleXY, scaleZ), 0.1f).SetLoops(2, LoopType.Yoyo));
            sequence.Append(img.DOFade(0f, fadeTime));
        }



        static public void PoopUpTextTween(Text text, Color color)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(text.DOColor(color, 0.1f));
            sequence.Join(text.transform.DOScale(new Vector3(3f, 3f, 1f), 0.1f));
            sequence.SetLoops(2, LoopType.Yoyo);
        }
    }