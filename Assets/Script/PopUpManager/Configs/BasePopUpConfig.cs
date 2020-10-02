using System;

namespace PopUp
{
    [System.Serializable]
    public abstract class BasePopUpConfig
    {
    }

    public abstract class GenericPopUpConfig<T> : BasePopUpConfig where T : GenericPopUpConfig<T>
    {
        public T AsActualType() => this as T;
    }
    
    [System.Serializable]
    public class PopUpOption
    {
        public string name;
        public Action callback;
    }
    
    [System.Serializable]
    public class OneOptionConfig : GenericPopUpConfig<OneOptionConfig> 
    {
        public string MainText;
        public PopUpOption option;

        public OneOptionConfig(string mainText, PopUpOption option)
        {
            MainText = mainText;
            this.option = option;
        }

        public OneOptionConfig()
        {
        }
    }
        
    [System.Serializable]
    public class TwoOptionsConfig : GenericPopUpConfig<TwoOptionsConfig> 
    {
        public string MainText;
        public PopUpOption confirm;
        public PopUpOption cancel;

        public TwoOptionsConfig(string mainText, PopUpOption confirm, PopUpOption cancel)
        {
            MainText = mainText;
            this.confirm = confirm;
            this.cancel = cancel;
        }

        public TwoOptionsConfig()
        {
        }
    }
    
    [System.Serializable]
    public class ThreeOptionsConfig : GenericPopUpConfig<ThreeOptionsConfig> 
    {
        public string MainText;
        public PopUpOption OptionA;
        public PopUpOption OptionB;
        public PopUpOption OptionC;

        public ThreeOptionsConfig(string mainText, PopUpOption optionA, PopUpOption optionB, PopUpOption optionC)
        {
            MainText = mainText;
            OptionA = optionA;
            OptionB = optionB;
            OptionC = optionC;
        }

        public ThreeOptionsConfig()
        {
        }
    }
}
