using System;
using PopUp;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("PopUp/1 Option")]
public class OneOptionPopUp : GenericPopUp<OneOptionConfig>
{
    public Text mainTxt;
    public Text buttonTxt;
    private Action callback;
    
    public override void Initialize(OneOptionConfig nConfig)
    {
        mainTxt.text = nConfig.MainText;
        buttonTxt.text = nConfig.option.name;
        callback = nConfig.option.callback;
    }

    protected override void _Open(Action onDone)
    {
        //PopUpManager.Show(new Config("MainText", "ButtonText", ()=> Debug.LogWarning("ASDA")));
        gameObject.SetActive(true);
        onDone?.Invoke();
    }

    protected override void _Close(Action onDone)
    {
        onDone?.Invoke();
        callback = null;
        gameObject.SetActive(false);
    }

    public void OnConfirmPressed()
    {
        callback?.Invoke();
        Close();
    }
    
    [System.Serializable]
    public class Clip : GenericPopUpClip<OneOptionConfig,OneOptionPopUp>
    {
        public UnityEvent OnPressed;
        public override void Show(OneOptionConfig config)
        {
            config.option.callback -= OnPressed.Invoke;
            config.option.callback += OnPressed.Invoke;
            base.Show(config);
        }

        public override void Enqueue(OneOptionConfig config)
        {
            config.option.callback -= OnPressed.Invoke;
            config.option.callback += OnPressed.Invoke;
            base.Enqueue(config);
        }
    }
}