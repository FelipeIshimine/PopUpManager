using System;
using NaughtyAttributes;
using UnityEngine;

namespace GenericAnimationCore
{
    [System.Serializable]
    public class Position : IAnimationModule
    {
        public Transform target;

        public bool useWorldPosition = true;
        public bool useFixedStartPosition = false;
        [ShowIf("useFixedStartPosition")] public Vector3 startPosition = Vector3.zero;
        public Vector3 endPosition = Vector3.right;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        public Action<float> GetAnimationStep()
        {
            Vector3 transformStartPosition = (useFixedStartPosition) ? this.startPosition :
                (useWorldPosition) ? target.transform.position : target.transform.localPosition;
            
            if(useWorldPosition)
                return t => target.transform.position = Vector3.LerpUnclamped(transformStartPosition, endPosition, curve.Evaluate(t));
            else
                return t => target.transform.localPosition = Vector3.LerpUnclamped(transformStartPosition, endPosition, curve.Evaluate(t));
        }
    }
}