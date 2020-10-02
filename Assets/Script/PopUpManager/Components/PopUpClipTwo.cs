using PopUp;
using UnityEngine.Events;

[System.Serializable]
public class PopUpClipTwo : PopUpClip<TwoOptionsConfig,TwoOptionPopUp>
{
    public UnityEvent OnConfirm;
    public UnityEvent OnCancel;

    public override void Show()
    {
        Config.confirm.callback -= OnConfirm.Invoke;
        Config.confirm.callback += OnConfirm.Invoke;
        Config.cancel.callback -= OnCancel.Invoke;
        Config.cancel.callback += OnCancel.Invoke;
        base.Show();
    }
    
    public override void Enqueue()
    {
        Config.confirm.callback -= OnConfirm.Invoke;
        Config.confirm.callback += OnConfirm.Invoke;
        Config.cancel.callback -= OnCancel.Invoke;
        Config.cancel.callback += OnCancel.Invoke;
        base.Enqueue();
    }
}