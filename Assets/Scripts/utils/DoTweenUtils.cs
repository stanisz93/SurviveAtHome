    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    class DoTweenUtils
    {
    
        static public void PoopUpImage(Image img, float scaleXY=1.5f, float scaleZ=1f)
        {
            img.transform.DOScale(new Vector3(scaleXY, scaleXY, scaleZ), 0.1f).SetLoops(2, LoopType.Yoyo);
        }

        static public void PoopUpTextTween(Text text, Color color)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(text.DOColor(color, 0.1f));
            sequence.Join(text.transform.DOScale(new Vector3(3f, 3f, 1f), 0.1f));
            sequence.Join(text.transform.DOScale(new Vector3(3f, 3f, 1f), 0.1f));
            sequence.SetLoops(2, LoopType.Yoyo);
        }
    }