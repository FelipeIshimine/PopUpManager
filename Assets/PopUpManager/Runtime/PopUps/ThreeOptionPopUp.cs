using System;
using UnityEngine;
using UnityEngine.UI;
using PopUp;
using UnityEngine.Events;

[AddComponentMenu("PopUp/3 Options")]
public class ThreeOptionPopUp : GenericPopUp<PopUp.ThreeOptionsConfig>
{
    public Text mainTxt;
    public Text optionA;
    public Text optionB;
    public Text optionC;

    private PopUp.ThreeOptionsConfig config;

    public override void Initialize(PopUp.ThreeOptionsConfig nConfig)
    {
        this.config = nConfig;
        mainTxt.text = nConfig.MainText;
        optionA.text = nConfig.OptionA.name;
        optionB.text = nConfig.OptionB.name;
        optionC.text = nConfig.OptionC.name;
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

    public void OnOptionA()
    {
        config.OptionA.callback?.Invoke();
        Close();
    }

    public void OnOptionB()
    {
        config.OptionB.callback?.Invoke();
        Close();
    }

    public void OnOptionC()
    {
        config.OptionC.callback?.Invoke();
        Close();
    }

    [System.Serializable]
    public class Clip : GenericPopUpClip<ThreeOptionsConfig, ThreeOptionPopUp>
    {
        public UnityEvent OnOptionA;
        public UnityEvent OnOptionB;
        public UnityEvent OnOptionC;

        public override void Show(ThreeOptionsConfig config)
        {
            Register(config);
            base.Show(config);
        }

        public override void Enqueue(ThreeOptionsConfig config)
        {
            Register(config);
            base.Enqueue(config);
        }

        private void Register(ThreeOptionsConfig config)
        {
            config.OptionA.callback -= OnOptionA.Invoke;
            config.OptionA.callback += OnOptionA.Invoke;
            config.OptionB.callback -= OnOptionB.Invoke;
            config.OptionB.callback += OnOptionB.Invoke;
            config.OptionC.callback -= OnOptionC.Invoke;
            config.OptionC.callback += OnOptionC.Invoke;
        }
    }
}