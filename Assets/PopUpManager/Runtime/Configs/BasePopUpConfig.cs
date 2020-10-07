using System;

namespace PopUp
{
    [System.Serializable]
    public abstract class BasePopUpConfig
    {
    }

   [System.Serializable]
    public class PopUpOption
    {
        public string name;
        public Action callback;

        public PopUpOption(string name, Action callback)
        {
            this.name = name;
            this.callback = callback;
        }
    }
}
