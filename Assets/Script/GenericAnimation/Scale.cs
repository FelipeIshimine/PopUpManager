using System;
using NaughtyAttributes;
using UnityEngine;

namespace GenericAnimationCore
{
    [System.Serializable]
    public class Scale : IAnimationModule
    {
        public Transform target;

        public bool useFixedStartSize = false;
        [ShowIf("useFixedStartSize")] public Vector3 startSize = Vector3.zero;
        public Vector3 endSize = Vector3.one;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public float mult = 1;

        public Scale()
        {
        }

        public Action<float> GetAnimationStep()
        {
            Vector3 targetStartSize = (useFixedStartSize) ? this.startSize : target.localScale;
            return t => target.localScale = Vector3.LerpUnclamped(targetStartSize, endSize, curve.Evaluate(t))*mult;
        }
    }
}