namespace PopUp
{
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
}