using System;
using System.Collections;
using System.Collections.Generic;
using PopUp;
using MarketSystem;
using UnityEngine.UI;

public class TwoOptionsOneImagePopUp : GenericPopUp<PopUp.TwoOptionsOneImageConfig>
{
    public Text mainText;
    public Image img;
    public Text firstOptionText;
    public Text secondOptionText;

    private TwoOptionsOneImageConfig config;

    public override void Initialize(TwoOptionsOneImageConfig nConfig)
    {
        this.config = nConfig;
        img.sprite = nConfig.sprite;
        mainText.text = nConfig.mainText;
        firstOptionText.text = nConfig.firstOption.name;
        secondOptionText.text = nConfig.secondOption.name;
    }

    public void FirstOption()
    {
        config.firstOption.callback?.Invoke();
        Close();
    }

    public void SecondOption()
    {
        config.secondOption.callback?.Invoke();
        Close();
    }


    [System.Serializable]
    public class Clip : GenericPopUpClip<TwoOptionsOneImageConfig, TwoOptionsOneImagePopUp>
    {

    }
}
