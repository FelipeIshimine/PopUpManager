using NaughtyAttributes;
using UnityEngine;

public class PopUpClipComponent : MonoBehaviour
{
    [SerializeReference, SerializeReferenceButton] public BasePopUpClip basePopUpClip;

    [Button]
    public void Show() => basePopUpClip.Show();
    
    [Button]
    public void Enqueue() => basePopUpClip.Enqueue();
}