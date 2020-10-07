using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace GenericAnimationCore
{
    [System.Serializable]
    public class ImageColor : IAnimationModule
    {
        public Image targetImage;

        public bool useFixedStartValue = false;
        [ShowIf("useFixedStartValue")] public Color startValue = Color.clear;
        public Color endValue = Color.white;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public Action<float> GetAnimationStep()
        {
            Color startTargetColor = (useFixedStartValue) ? this.startValue : targetImage.color;
            return t => targetImage.color = Color.LerpUnclamped(startTargetColor, endValue, curve.Evaluate(t));
        }
    }
}