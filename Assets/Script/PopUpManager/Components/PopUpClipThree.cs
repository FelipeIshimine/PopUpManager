using PopUp;
using UnityEngine.Events;

[System.Serializable]
public class PopUpClipThree : PopUpClip<ThreeOptionsConfig,ThreeOptionPopUp>
{
    public UnityEvent OnOptionA;
    public UnityEvent OnOptionB;
    public UnityEvent OnOptionC;


    public override void Show()
    {
        Register();
        base.Show();
    }

    public override void Enqueue()
    {
        Register();
        base.Enqueue();
    }
    
    private void Register()
    {
        Config.OptionA.callback += OnOptionA.Invoke;
        Config.OptionA.callback -= OnOptionA.Invoke;
        Config.OptionB.callback += OnOptionB.Invoke;
        Config.OptionB.callback -= OnOptionB.Invoke;
        Config.OptionC.callback += OnOptionC.Invoke;
        Config.OptionC.callback -= OnOptionC.Invoke;
    }
}