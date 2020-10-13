using System;
using MarketSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateItemTypeWindow : EditorWindow
{
    public float rotationAmount = 0.33f;
    public string selected = "";

    private TextField _textField;
    private Action _onDone;

    private static CreateItemTypeWindow window;
    private bool _createBaseType = true;
    private bool CreateBaseType => _createBaseType;
    public static void Show(Action onDone)
    {
        if(window == null)
            window = CreateInstance(typeof(CreateItemTypeWindow)) as CreateItemTypeWindow;
        
        Debug.Assert(window != null, nameof(window) + " != null");
        window.titleContent.text = "New Item Type";
        window.Focus();
        window.ShowUtility();
        window._onDone = onDone;
    }

    private void OnEnable()
    {
        VisualElement root = rootVisualElement;

        var visualTree = Resources.Load<VisualTreeAsset>("NewItemType_Main");

        visualTree.CloneTree(root);

        _textField = rootVisualElement.Q<TextField>("TextField");
        Button acceptButton = rootVisualElement.Q<Button>("AcceptB");
        Button cancelButton = rootVisualElement.Q<Button>("CancelB");
        Toggle dontCreateBaseType = rootVisualElement.Q<Toggle>("ToggleB");
        
        dontCreateBaseType.RegisterValueChangedCallback(OnToggle);
        dontCreateBaseType.SetValueWithoutNotify(!_createBaseType);
        
        acceptButton.clicked += () => SelectName(_textField.value);
        cancelButton.clicked += Close;
    }

    private void OnToggle(ChangeEvent<bool> evt)
    {
        _createBaseType = !evt.newValue;
    }

    private void SelectName(string newName)
    {
        ItemScriptsCreator.CreateNewItemType(newName, CreateBaseType);
        _onDone.Invoke();
        Close();
    }

    private void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SelectName(_textField.value);
        else if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }
}