using UnityEngine;

namespace PopUp
{
    [System.Serializable]
    public class TwoOptionsOneImageConfig : GenericPopUpConfig<TwoOptionsOneImageConfig>
    {
        public string mainText;
        public Sprite sprite;
        public PopUpOption firstOption, secondOption;
        public TwoOptionsOneImageConfig(string mainText, Sprite sprite, PopUpOption firstOption, PopUpOption secondOption)
        {
            this.mainText = mainText;
            this.sprite = sprite;
            this.firstOption = firstOption;
            this.secondOption = secondOption;
        }

    }
}

