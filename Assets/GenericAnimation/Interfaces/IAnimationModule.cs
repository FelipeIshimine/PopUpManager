using System;

namespace GenericAnimationCore
{
    public interface IAnimationModule
    {
        Action<float> GetAnimationStep();
    }
}