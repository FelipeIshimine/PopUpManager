using System;
using UnityEngine.UI;

public class ConfirmPopUp : GenericPopUp<ConfirmPopUp.Config>
{
    public Text mainTxt;
    public Text confirmText;
    public Text cancelText;

    private Config _config;
        
    [System.Serializable]
    public class Config : PopUpConfig<ConfirmPopUp, Config>
    {
        public readonly string MainText;
        public readonly string ConfirmText;
        public readonly string CancelText;
        public readonly Action OnConfirm;
        public readonly Action OnCancel;

        public Config(string mainText, string confirmText, string cancelText, Action onConfirm, Action onCancel)
        {
            MainText = mainText;
            ConfirmText = confirmText;
            CancelText = cancelText;
            OnConfirm = onConfirm;
            OnCancel = onCancel;
        }
    }

    public override void Initialize(Config nConfig)
    {
        _config = nConfig;
        mainTxt.text = nConfig.MainText;
        confirmText.text = nConfig.ConfirmText;
        cancelText.text = nConfig.CancelText;
    }

    protected override void _Open()
    {
        gameObject.SetActive(true);
        OnOpenDone?.Invoke();
    }

    protected override void _Close()
    {
        gameObject.SetActive(false);
        OnCloseDone?.Invoke();
        _config = null;
    }

    public void ConfirmPressed()
    {
        _config.OnConfirm?.Invoke();
        Close();
    }
    
    public void CancelPressed()
    {
        _config.OnCancel?.Invoke();
        Close();
    }
}