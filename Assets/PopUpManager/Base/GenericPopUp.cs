using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class GenericPopUp<T> : BasePopUp, IInitialize<T> where T : PopUp.BasePopUpConfig
{
    public abstract void Initialize(T nConfig);
    public override Type GetConfigType() => typeof(T);
  
}