using PopUp;

public abstract class PopUpClip<T,B> :BasePopUpClip where T : BasePopUpConfig where B : BasePopUp
{
    public T Config;
    public B Prefab;
    
    public override void Show() => PopUpManager.Show((Prefab==null)?null:Prefab.gameObject, Config);
    public override void Enqueue() => PopUpManager.Enqueue((Prefab==null)?null:Prefab.gameObject, Config);
}