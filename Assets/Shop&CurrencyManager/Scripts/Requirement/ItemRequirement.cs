namespace ShopSystem
{
    [System.Serializable]
    public class ItemRequirement
    {
        public Item item;
        public int ammount;
        public bool consumes = false;
    }
}