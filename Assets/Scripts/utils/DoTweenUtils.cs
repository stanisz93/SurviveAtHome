    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    class DoTweenUtils
    {
    
        static public void PoopUpImage(Image img, float scale=1.5f)
        {
            img.transform.DOScale(new Vector3(scale, scale, 1f), 0.1f).SetLoops(2, LoopType.Yoyo);
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