using PopUp;

public abstract class GenericPopUpClip<T,TB> :BasePopUpClip where T : GenericPopUpConfig<T> where TB : GenericPopUp<T>
{
    public TB Prefab;
    public virtual void Show(T config) => PopUpManager.Show((Prefab==null)?null:Prefab, config);
    public virtual void Enqueue(T config) => PopUpManager.Enqueue((Prefab==null)?null:Prefab, config);
}