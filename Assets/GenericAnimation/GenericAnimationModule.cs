using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenericAnimationCore
{
    [System.Serializable]
    public class GenericAnimationModule
    {
        public float duration = 1;
        private IEnumerator rutine;

        [SerializeReference, SerializeReferenceButton]
        public List<IAnimationModule> AnimationModules = new List<IAnimationModule>();

        private Action<float> animationStep;

        public bool HasAnimations => AnimationModules.Count > 0 && duration > 0;

        private void InitializeAnimationStep()
        {
            animationStep = null;
            foreach (IAnimationModule item in AnimationModules)
                animationStep += item.GetAnimationStep();
        }

        public void Play(MonoBehaviour source, Action callback = null)
        {
            source.gameObject.SetActive(true);
            InitializeAnimationStep();
            Stop(source);
            rutine = AnimationRutine(callback);
            source.StartCoroutine(rutine);
        }

        public void Stop(MonoBehaviour source)
        {
            if (rutine != null) source.StopCoroutine(rutine);
        }

        public IEnumerator AnimationRutine(Action callback)
        {
            float t = 0;
            do
            {
                t += Time.deltaTime / duration;
                animationStep.Invoke(t);
                yield return null;
            } while (t < 1);
            callback?.Invoke();
        }

        public void AddAnimationModule(IAnimationModule nAnimationModule)
        {
            AnimationModules.Add(nAnimationModule);
        }
    }

}