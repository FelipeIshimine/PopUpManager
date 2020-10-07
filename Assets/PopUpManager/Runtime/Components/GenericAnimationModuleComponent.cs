using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace GenericAnimationCore
{
   public class GenericAnimationModuleComponent : MonoBehaviour
   {
      public GenericAnimationModule animationModule;

      [Button]
      public void Play()
      {
         animationModule.Play(this);
      }
      
      [Button]
      public void Stop()
      {
         animationModule.Stop(this);
      }

   }
}