using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "PopUpManager", fileName = "PopUpManager")]
public class PopUpManager : ScriptableObject
{
    #region Singleton
    private const string FileName = "PopUpManager";
    private static PopUpManager _instance;
    public static PopUpManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindOrCreate();
            return _instance;
        }
    }
    private static PopUpManager FindOrCreate()
    {
        PopUpManager popUpManager = Resources.Load<PopUpManager>(FileName);
#if UNITY_EDITOR
        if (!popUpManager)
        {
            popUpManager = CreateInstance<PopUpManager>();
            AssetDatabase.CreateAsset(popUpManager, "Assets/Resources/" + FileName +".asset");
            AssetDatabase.SaveAssets();
            
            Debug.LogWarning($"popUpManager is null:{popUpManager==null}");
        }
#endif
        return popUpManager;
    }
    #endregion
    
    public List<GameObject> prefabs = new List<GameObject>();
    [ShowInInspector] private Dictionary<string,GameObject> popUps;
    [ShowInInspector]
    private Dictionary<string, GameObject> PopUps
    {
        get
        {
            if (popUps == null)
                InitializeDictionary();
            return popUps;
        }
    }

    public Stack<BasePopUp> ActivePopUps = new Stack<BasePopUp>();
    
    public Queue<BasePopUp> waitingPopUps = new Queue<BasePopUp>();

    [Button]
    public void InitializeDictionary()
    {
        popUps = new Dictionary<string, GameObject>();
        for (int i = 0; i < prefabs.Count; i++)
        {
            BasePopUp popUp = prefabs[i].GetComponent<BasePopUp>();
            popUps.Add(popUp.GetType().Name, popUp.gameObject);
            Debug.Log($"Added: {popUp.GetType().Name} => {popUp.gameObject}");
        }
    }
    
    public static void Show<T>(T config) where T : BasePopUpConfig
    {
        Type t = config.GetPopUpType();

        GameObject prefab = Instance.PopUps[t.Name];
        GameObject go = Instantiate(prefab);
        
        IInitialize<T> popUp = go.GetComponent<IInitialize<T>>();
        popUp.Initialize(config);

        Instance.Show((BasePopUp)popUp);
    }

    public void Show(BasePopUp popUp)
    {
        ActivePopUps.Push(popUp);
        popUp.OnCloseDone += OnPopUpClose;
        popUp.OnCloseDone += ()=> Destroy(popUp.gameObject);
        
        popUp.Open();
    }

    private void ShowNextInQueue()
    {
        if(waitingPopUps.Count > 0)
            Show(waitingPopUps.Dequeue());
    }
        
    public void Enqueue(BasePopUp basePopUp)
    {
        waitingPopUps.Enqueue(basePopUp);
        if (ActivePopUps.Count == 0) ShowNextInQueue();
    }

    private void OnPopUpClose()
    {
        ActivePopUps.Pop();
        if (ActivePopUps.Count == 0) ShowNextInQueue();
    }

    [Button]
    public void TestShow(ConfirmPopUp.Config config) => Show(config);
    
    [Button]
    public void TestShow(MessagePopUp.Config config) => Show(config);
}

public abstract class BasePopUp : MonoBehaviour
{
    public Action OnOpenDone;
    public Action OnCloseDone;
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        if (IsOpen)
        {
            Debug.LogWarning($"{this} already open");
            return;
        }

        IsOpen = true;
        _Open();
    }

    public void Close()
    {
        if (!IsOpen)
        {
            Debug.LogWarning($"{this} already close");
            return;
        }

        IsOpen = false;
        _Close();
    }

    protected abstract void _Open();
    protected abstract void _Close();
}

public abstract class GenericPopUp<T> : BasePopUp, IInitialize<T> where T : BasePopUpConfig
{
     public abstract void Initialize(T nConfig);
}

public abstract class PopUpConfig<T,TMySelf> : BasePopUpConfig where T : GenericPopUp<TMySelf> where TMySelf : PopUpConfig<T,TMySelf>
{
    public override Type GetPopUpType() => typeof(T);
}

public abstract class BasePopUpConfig
{
    public abstract Type GetPopUpType();
}

public interface IInitialize<in T>
{
    void Initialize(T nConfig);
}