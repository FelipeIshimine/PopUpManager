using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using PopUp;
using UnityEngine;
using UnityEditor;

public class MonoTester : MonoBehaviour
{
    [SerializeReference,SerializeReferenceButton] public BasePopUpConfig message;
    public BasePopUp overridePopUp;

    [Button()]
    public void ShowDefault() => PopUpManager.Show(message);

    [Button]
    public void ShowOverride() =>  PopUpManager.Show(overridePopUp.gameObject, message);
    
    [Button]
    public void EnqueueDefault() => PopUpManager.Enqueue(message);
    
    [Button]
    public void EnqueueOverride() => PopUpManager.Enqueue(overridePopUp.gameObject, message);
    
    [SerializeReference,SerializeReferenceButton] public BasePopUpClip PopUpClip;

    [Button()]
    public void ShowClip() => PopUpClip.Show();
    
    [Button]
    public void EnqueueClip() => PopUpClip.Enqueue();
    
}
