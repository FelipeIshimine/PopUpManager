namespace PopUp
{
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