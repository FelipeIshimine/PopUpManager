using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("PopUp/1 Option")]
public class OneOptionPopUp : GenericPopUp<PopUp.OneOptionConfig>
{
    public Text mainTxt;
    public Text buttonTxt;
    private Action callback;
    
    public override void Initialize(PopUp.OneOptionConfig nConfig)
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
}