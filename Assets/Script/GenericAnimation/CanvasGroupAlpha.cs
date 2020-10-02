using System;
using NaughtyAttributes;
using UnityEngine;

namespace GenericAnimationCore
{
    [System.Serializable]
    public class CanvasGroupAlpha : IAnimationModule
    {
        public CanvasGroup target;
        public bool useFixedStartColor = false;
        [ShowIf("useFixedStartValue")] public float startValue = 0;
        public float endValue = 1;
        [SerializeField] public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public Action<float> GetAnimationStep()
        {
            float startTargetlpha = (useFixedStartColor) ? startValue : target.alpha;
            return t =>target.alpha = Mathf.LerpUnclamped(startTargetlpha, endValue, curve.Evaluate(t));
        }
    }
}    