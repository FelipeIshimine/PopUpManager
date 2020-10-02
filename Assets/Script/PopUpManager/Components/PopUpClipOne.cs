using System.Collections;
using System.Collections.Generic;
using PopUp;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PopUpClipOne : PopUpClip<OneOptionConfig,OneOptionPopUp>
{
    public UnityEvent OnPressed;


    public override void Show()
    {
        Config.option.callback -= OnPressed.Invoke;
        Config.option.callback += OnPressed.Invoke;
        base.Show();
    }
    
    public override void Enqueue()
    {
        Config.option.callback -= OnPressed.Invoke;
        Config.option.callback += OnPressed.Invoke;
        base.Enqueue();
    }
}