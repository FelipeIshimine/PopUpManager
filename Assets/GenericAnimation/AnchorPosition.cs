using System;
using NaughtyAttributes;
using UnityEngine;

namespace GenericAnimationCore
{
    [System.Serializable]
    public class AnchorPosition : IAnimationModule
    {
        public RectTransform target;

        public bool useFixedStartPosition = false;
        [ShowIf("useFixedStartPosition")] public Vector2 startPosition = Vector2.zero;
        public Vector2 endPosition = Vector2.right;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public Action<float> GetAnimationStep()
        {
            Vector2 transformStartPosition = (useFixedStartPosition) ? (Vector2)this.startPosition : target.anchoredPosition;
            return t => target.transform.localPosition = Vector2.LerpUnclamped(transformStartPosition, endPosition, curve.Evaluate(t));
        }
    }
}