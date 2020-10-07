namespace PopUp
{
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
}