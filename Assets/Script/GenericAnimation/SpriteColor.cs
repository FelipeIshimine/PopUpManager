using System;
using NaughtyAttributes;
using UnityEngine;

namespace GenericAnimationCore
{
    [System.Serializable]
    public class SpriteColor : IAnimationModule
    {
        public SpriteRenderer targetImage;

        public bool useFixedStartValue = false;
        [ShowIf("useFixedStartValue")] public Color startColor = Color.clear;
        public Color endColor = Color.white;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public Action<float> GetAnimationStep()
        {
            Color startTargetColor = (useFixedStartValue) ? this.startColor : targetImage.color;
            return t => targetImage.color = Color.LerpUnclamped(startTargetColor, endColor, curve.Evaluate(t));
        }
    }
}