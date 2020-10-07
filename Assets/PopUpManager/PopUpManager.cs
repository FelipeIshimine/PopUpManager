using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Managers/PopUp", fileName = "PopUpManager")]
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

    public List<GameObject> defaultPopUps = new List<GameObject>();
    private Dictionary<string,GameObject> popUps;
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
    public Queue<BasePopUp> WaitingPopUps = new Queue<BasePopUp>();

    public void InitializeDictionary()
    {
        popUps = new Dictionary<string, GameObject>();
        for (int i = 0; i < defaultPopUps.Count; i++)
        {
            BasePopUp popUp = defaultPopUps[i].GetComponent<BasePopUp>();
            popUps.Add(popUp.GetConfigType().FullName ?? string.Empty, popUp.gameObject);
            Debug.Log($"Added: {popUp.GetConfigType().FullName} => {popUp.gameObject}");
        }
    }

    #region Show

    public static void Show<T>(T config) where T : PopUp.BasePopUpConfig
    {
        Type t = config.GetType();
        GameObject prefab = Instance.PopUps[t.FullName];
        if(prefab == null)
            throw new Exception($"No se encontro un PopUp Default adecuado para {t.FullName}");
              
        Show(prefab, config);
    }

    public static void Show<T>(GameObject prefab, T config) where T : PopUp.BasePopUpConfig
    {
        if (prefab == null)
        {
            Show(config);
            return;
        }
          
        GameObject go = Instantiate(prefab);
        dynamic d = config;
        Instance._Show(go.GetComponent<BasePopUp>(), d.AsActualType());
    }

    private void _Show<T>(BasePopUp basePopUp, T config) where T : PopUp.BasePopUpConfig
    {
        if (!(basePopUp is IInitialize<T> popUp))
            Debug.LogError($"El popUp {basePopUp} no se configura con un {typeof(T).FullName} necesita un {basePopUp.GetConfigType().FullName}");
        else
            popUp.Initialize(config);
        
        _Show(basePopUp);
    }

    private void _Show(BasePopUp basePopUp)
    {
        ActivePopUps.Push(basePopUp);

        basePopUp.RegisterOnCloseDone(OnPopUpClose);
        basePopUp.RegisterOnCloseDone(() => Destroy(basePopUp.gameObject));
        
        basePopUp.Open();
    }

    private void ShowNextInQueue()
    {
        if(WaitingPopUps.Count > 0)
            _Show(WaitingPopUps.Dequeue());
    }
    
    #endregion

    #region Enqueue
    
    public static void Enqueue<T>(T config) where T : PopUp.BasePopUpConfig
    {
        Type t = config.GetType();
        GameObject prefab = Instance.PopUps[t.FullName];
        if(prefab == null)
            throw new Exception($"No se encontro un PopUp Defatul adecuado para {t.FullName}");
        Enqueue(prefab, config);
    }
    
    public static void Enqueue<T>(GameObject prefab, T config) where T : PopUp.BasePopUpConfig
    {
        if (prefab == null)
        {
            Enqueue(config);
            return;
        }
        GameObject go = Instantiate(prefab);
        dynamic d = config;
        Instance._Enqueue(go.GetComponent<BasePopUp>(), d.AsActualType());
    }
    
    private void _Enqueue<T>(BasePopUp basePopUp, T config) where T : PopUp.BasePopUpConfig
    {
        IInitialize<T> popUp = basePopUp as IInitialize<T>;
        if (popUp == null)
            Debug.LogError($"El popUp {basePopUp} no se configura con un {typeof(T).FullName} necesita un {basePopUp.GetConfigType().FullName}");
        else
            popUp.Initialize(config);
        _Enqueue(basePopUp);
    }
    
    public void _Enqueue(BasePopUp basePopUp)
    {
        WaitingPopUps.Enqueue(basePopUp);
        if (ActivePopUps.Count == 0) ShowNextInQueue();
    }
    
    #endregion

    private void OnPopUpClose()
    {
        ActivePopUps.Pop();
        if (ActivePopUps.Count == 0) ShowNextInQueue();
    }
}