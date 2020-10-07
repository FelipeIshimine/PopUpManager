using PopUp;

public abstract class PopUpClip<T,B> :BasePopUpClip where T : GenericPopUpConfig<T> where B : GenericPopUp<T>
{
    public T Config;
    public B Prefab;
    
    public override void Show() => PopUpManager.Show((Prefab==null)?null:Prefab, Config);
    public override void Enqueue() => PopUpManager.Enqueue((Prefab==null)?null:Prefab, Config);
}