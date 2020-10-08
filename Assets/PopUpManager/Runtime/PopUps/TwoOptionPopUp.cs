using System;
using UnityEngine;
using UnityEngine.UI;
using PopUp;
using UnityEngine.Events;

[AddComponentMenu("PopUp/2 Options")]
public class TwoOptionPopUp : GenericPopUp<PopUp.TwoOptionsConfig>
{
    public Text mainTxt;
    public Text confirmText;
    public Text cancelText;

    private PopUp.TwoOptionsConfig config;

    public override void Initialize(PopUp.TwoOptionsConfig nConfig)
    {
        this.config = nConfig;
        mainTxt.text = nConfig.MainText;
        confirmText.text = this.config.confirm.name;
        cancelText.text = this.config.cancel.name;
    }

    protected override void _Open(Action onDone)
    {
        gameObject.SetActive(true);
        onDone?.Invoke();
    }

    protected override void _Close(Action onDone)
    {
        gameObject.SetActive(false);
        onDone?.Invoke();
        config = null;
    }

    public void ConfirmPressed()
    {
        config.confirm.callback?.Invoke();
        Close();
    }

    public void CancelPressed()
    {
        config.cancel.callback?.Invoke();
        Close();
    }

    [System.Serializable]
    public class Clip : GenericPopUpClip<TwoOptionsConfig, TwoOptionPopUp>
    {
        public UnityEvent OnConfirm;
        public UnityEvent OnCancel;

        private void Register(TwoOptionsConfig config)
        {
            config.confirm.callback -= OnConfirm.Invoke;
            config.confirm.callback += OnConfirm.Invoke;
            config.cancel.callback -= OnCancel.Invoke;
            config.cancel.callback += OnCancel.Invoke;
        }

        public override void Show(TwoOptionsConfig config)
        {
            Register(config);
            base.Show(config);
        }

        public override void Enqueue(TwoOptionsConfig config)
        {
            Register(config);
            base.Enqueue(config);
        }
    }
}