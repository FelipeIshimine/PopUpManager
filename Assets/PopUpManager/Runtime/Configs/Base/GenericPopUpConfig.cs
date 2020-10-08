namespace PopUp
{
    public abstract class GenericPopUpConfig<T> : BasePopUpConfig where T : GenericPopUpConfig<T>
    {
        public T AsActualType() => this as T;
    }
}