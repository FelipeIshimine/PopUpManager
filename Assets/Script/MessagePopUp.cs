using System;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopUp : GenericPopUp<MessagePopUp.Config>, IInitialize<MessagePopUp.Config>
{
    public Text mainTxt;
    public Text buttonTxt;
    private Action callback;

    [System.Serializable]
    public class Config : PopUpConfig<MessagePopUp, Config>
    {
        public readonly string MainText;
        public readonly string ButtonText;
        public Action Callback;

        public Config(string mainText, string buttonText, Action callback)
        {
            MainText = mainText;
            ButtonText = buttonText;
            Callback = callback;
        }
    }

    public override void Initialize(Config nConfig)
    {
        mainTxt.text = nConfig.MainText;
        buttonTxt.text = nConfig.ButtonText;
        callback = nConfig.Callback;
    }

    protected override void _Open()
    {
        //PopUpManager.Show(new Config("MainText", "ButtonText", ()=> Debug.LogWarning("ASDA")));
        gameObject.SetActive(true);
        OnOpenDone?.Invoke();
    }

    protected override void _Close()
    {
        gameObject.SetActive(false);
        OnCloseDone?.Invoke();
        callback = null;
    }

    public void OnConfirmPressed()
    {
        callback?.Invoke();
        Close();
    }
}