using System;
using System.Collections.Generic;
using System.Linq;
using MarketSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Object = UnityEngine.Object;

public class MarketSystemWindow : EditorWindow    
{
    private Object _selection;
    private string _filterValue = string.Empty;

    private const int leftPanelMaxWidth = 170;
    private bool _isTagFoldoutOpen;

    private static MarketManager MarketManager=>MarketManager.Instance;

    private BaseItem selectedItem;
    private BaseCatalog selectedCatalog;    
    private BaseProduct selectedProduct;

    private List<string> selectedItemCategories = new List<string>();
    private List<string> selectedProductCategories = new List<string>();

    private enum States{Items,Catalogs,Products}

    private States currentState;
    
    [MenuItem("Window/Ishimine/MarketManager %#k", priority = 1)]
    public static void ShowWindow()
    {
        MarketSystemWindow wnd = GetWindow<MarketSystemWindow>();
        wnd.titleContent = new GUIContent("MarketManager");
    }

    public void OnEnable()
    {
        VisualElement root = rootVisualElement;
        var visualTree = Resources.Load<VisualTreeAsset>("MarketManager_Main");
        visualTree.CloneTree(root);
        var styleSheet = Resources.Load<StyleSheet>("MarketManager_Style");
        root.styleSheets.Add(styleSheet);

        ToolbarButton button = root.Q<ToolbarButton>("ItemsButton");
        button.clicked += () => ChangeTab(States.Items);
        
        button = root.Q<ToolbarButton>("CatalogsButton");
        button.clicked += () => ChangeTab(States.Catalogs);
        
        button = root.Q<ToolbarButton>("ProductsButton");
        button.clicked += () => ChangeTab(States.Products);

        ToolbarMenu toolbarMenu = root.Q<ToolbarMenu>("ToolbarMenu");
        toolbarMenu.variant = ToolbarMenu.Variant.Default;
        toolbarMenu.menu.AppendAction("ScanForAssets", x=> ScanForAssets());
        
        toolbarMenu.menu.AppendAction("New Item Type", x=> OpenCreateItemType());

        Render();
    }

    private void OpenCreateItemType()
    {
        CreateItemTypeWindow.Show(Render);
    }

    private void ScanForAssets()
    {
        MarketManager.ScanForAssets();
        Render();
    }

    private void ChangeTab(States nState)
    {
        currentState = nState;
        Render();
    }

    private void Render()
    {
        rootVisualElement.Q<VisualElement>("body").Clear();
        switch (currentState)
        {
            case States.Items:
                RenderItemsPanel();
                break;
            case States.Catalogs:
                RenderCatalogsPanel();
                break;
            case States.Products:
                RenderProductsPanel();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region Catalogs

    private void RenderCatalogsPanel()
    {
        VisualElement body = rootVisualElement.Q<VisualElement>("body");
        VisualElement leftPanel = new VisualElement();
        body.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        body.Add(leftPanel);
        leftPanel.style.borderRightWidth = 2;
        leftPanel.style.borderRightColor = new StyleColor(new Color(.3f,.3f,.3f));
        leftPanel.style.maxWidth = 160;
        leftPanel.contentContainer.style.maxWidth = 200;
        
        VisualElement midPanel = new VisualElement();
        midPanel.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        midPanel.style.borderLeftWidth = 1;
        midPanel.style.borderLeftColor = new StyleColor(new Color(.33f,.33f,.33f));
        midPanel.style.flexGrow = 1;
        body.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        body.Add(midPanel);
        
        VisualElement rightPanel = new VisualElement();
        rightPanel.style.flexGrow = 1;
        body.Add(rightPanel);
        
        foreach (BaseCatalog catalog in MarketManager.Catalogs)
        {
            if (catalog == null) continue;
            leftPanel.Add(CreateSelectCatalogButton(catalog));
        }

        if (selectedCatalog == null) return;
            
        foreach (KeyValuePair<string, List<BaseProduct>> keyValuePair in MarketManager.GetItemsPerClassFromCatalog(selectedCatalog))
        {
            VisualElement container = new VisualElement();
            Label label = new Label(keyValuePair.Key.Replace("MarketSystem.", string.Empty));
            label.AddToClassList("ItemTypeLabel");
            container.Add(label);
            foreach (BaseProduct baseProduct in keyValuePair.Value)
                container.Add(CreateProductSelectionButton(baseProduct));
            
            midPanel.Add(container);
        }
    }
    #endregion

    #region Products Panel

    private void RenderProductsPanel()
    {
        VisualElement body = rootVisualElement.Q<VisualElement>("body");
        body.Clear();

        VisualElement upperPanel = new VisualElement {name = "upperPanel"};
        upperPanel.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        upperPanel.style.flexGrow = 1;

        ScrollView lowerPanel = new ScrollView {name = "lowerPanel"};
        lowerPanel.style.flexGrow = 1;

        VisualElement leftUpperPanel = new VisualElement {name = "leftUpperPanel"};
        leftUpperPanel.style.flexGrow = 1;
        leftUpperPanel.style.maxWidth = 300;
        
        VisualElement rightUpperPanel = new VisualElement {name = "rightUpperPanel"};
        rightUpperPanel.style.flexGrow = 1;
    
        
        List<Type> types = new List<Type>(GetAllSubclassTypes<BaseProduct>(false));
        List<string> names = new List<string>();
        for (int i = 0; i < types.Count; i++)
            names.Add(types[i].ToString());

        Dictionary<string, List<BaseProduct>> productsByClass = MarketManager.GetProductsByClass();

        Label categoriesLabel = new Label(){ text = "ITEM TYPES"};
        categoriesLabel.style.fontSize = 20;
        
        
        ListView selectionList = CreateListView(names, selectedProductCategories, OnProductTypeSelected, "Product");
    
        selectionList.style.flexGrow = 1;
        selectionList.style.height = 100;
        selectionList.style.width = 160;
        
        for (int i = 0; i < names.Count; i++)
        {
            if(!selectedProductCategories.Contains(names[i])) continue;
            
            ScrollView categoryContainer = new ScrollView();
            categoryContainer.style.flexGrow = 0;
            categoryContainer.style.flexShrink = 1;
            categoryContainer.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
            categoryContainer.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);
            
            categoryContainer.style.paddingBottom = 5;
            categoryContainer.style.paddingLeft = 10;
            categoryContainer.style.paddingRight = 10;
            
            categoryContainer.contentContainer.style.flexGrow = 1;
            categoryContainer.contentContainer.style.height = new StyleLength(StyleKeyword.Auto);
            categoryContainer.contentContainer.style.width = new StyleLength(StyleKeyword.Auto);
            categoryContainer.name = "categoryContainer";
            categoryContainer.contentContainer.name = "categoryContentContainer";
            
            Label label = new Label(RemoveUntilFirstPoint(names[i]).Replace("Product",string.Empty));
            label.style.maxWidth = new StyleLength(StyleKeyword.Auto);
            label.style.fontSize = 20;    
            label.style.paddingBottom = 10;
            label.style.paddingLeft = 10;
            
            VisualElement contentContainer = new VisualElement();
            contentContainer.style.borderBottomColor = new StyleColor(new Color(.2f,.2f,.2f));
            contentContainer.style.borderBottomWidth = new StyleFloat(1);
            contentContainer.style.paddingBottom = new StyleLength(10);
            contentContainer.style.alignContent = new StyleEnum<Align>(Align.FlexStart);
            contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            contentContainer.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);
            
            categoryContainer.Add(label);
            categoryContainer.Add(contentContainer);
            
            if (productsByClass.ContainsKey(names[i]))
            {
                foreach (BaseProduct baseProduct in productsByClass[names[i]])
                    contentContainer.Add(CreateProductSelectionButton(baseProduct));
            }

            Type currentType = types[i];

            Button addButton = new Button {text = "+"};
            addButton.clicked += () => CreateNewProduct(currentType);
            addButton.style.alignSelf = new StyleEnum<Align>(Align.Center);
            addButton.AddToClassList("AddButton");
            contentContainer.Add(addButton);
            lowerPanel.contentContainer.Add(categoryContainer);
        }
        
        leftUpperPanel.Add(selectionList);
        rightUpperPanel.Add(CreateSelectedProductView(selectedProduct));

        body.Add(upperPanel);
        body.Add(lowerPanel);
        
        upperPanel.Add(leftUpperPanel);
        upperPanel.Add(rightUpperPanel);
    }

    private void CreateNewProduct(Type type)
    {
        AssetDatabase.Refresh();
        Object uObject = CreateInstance(type);
        MarketSystem.MarketManager.Instance.AddNewProduct(uObject as BaseProduct);
        BaseProduct nItem = uObject as BaseProduct;
        
        int i = -1;
        string assetName;
        string baseName = RemoveUntilFirstPoint(type.Name);
        do
        {
            i++;
            assetName = $"New {baseName} {i}";
        } while (MarketManager.DoesProductExists(assetName));
        
        nItem.name = assetName;
        
        string filePath = MarketManager.CreateFoldersForAsset("Products", nItem);
        AssetDatabase.CreateAsset(nItem,filePath + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Render();
    }

    private void OnProductTypeSelected(bool value, string selectedProductType)
    {
        if (value && !selectedProductCategories.Contains(selectedProductType))
            selectedProductCategories.Add(selectedProductType);
        else if (!value && selectedProductCategories.Contains(selectedProductType))
            selectedProductCategories.Remove(selectedProductType);
        
        Render();
    }

    #endregion

    #region Items
    
    private void RenderItemsPanel()
    {
        VisualElement body = rootVisualElement.Q<VisualElement>("body");
        body.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        
        VisualElement upper = new VisualElement();
        upper.style.flexGrow = 1;
        upper.style.flexShrink = 1;
        upper.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        upper.name = "upper";
        upper.style.borderBottomColor = new StyleColor(new Color(.2f,.2f,.2f));
        upper.style.borderBottomWidth = new StyleFloat(1);
        body.Add(upper);

        ScrollView lower = new ScrollView();
        lower.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        lower.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);
        lower.style.flexGrow = 1;
        lower.style.flexShrink = 1;
        lower.name = "lower";

        body.Add(lower);
        
        ScrollView leftPanel = new ScrollView();
        leftPanel.style.flexGrow = 1;
        leftPanel.style.borderRightColor = new StyleColor(new Color(.2f,.2f,.2f));
        leftPanel.style.borderRightWidth = 1;
        leftPanel.style.maxWidth = 180;
        upper.Add(leftPanel);
        
        VisualElement rightPanel = new VisualElement();
        rightPanel.style.flexGrow = 1;
        rightPanel.style.flexShrink = 1;
        upper.Add(rightPanel);
        
        List<Type> types = new List<Type>(GetAllSubclassTypes<BaseItem>(false));
        List<string> names = new List<string>();
        for (int i = 0; i < types.Count; i++)
            names.Add(types[i].ToString());
        
        Dictionary<string, List<BaseItem>> itemsPerClass = MarketManager.GetItemsByClass();
        Label categoriesLabel = new Label(){ text = "ITEM TYPES"};
        categoriesLabel.style.fontSize = 20;
        ListView selectionList = CreateListView(names, selectedItemCategories, OnItemCategorySelected, "Item");

        selectionList.style.flexGrow = 1;
        selectionList.style.height = 100;
        selectionList.style.width = 100;
        
        Button allButton = new Button {text = "Select All"};
        allButton.style.width = 70;
        allButton.clicked += ()=> OnAllCategorySelected(names);
        selectionList.contentContainer.Insert(0,allButton);
        
        selectionList.Insert(0,allButton);
        
        //listView.RegisterValueChangedCallback(x=> OnCategoryFoldoutChange(listView));
        leftPanel.contentContainer.Add(categoriesLabel);
        leftPanel.contentContainer.Add(selectionList);

        rightPanel.Add(RenderSelectedItem());
        
        for (int i = 0; i < names.Count; i++)
        {
            if(!selectedItemCategories.Contains(names[i])) continue;
            ScrollView categoryContainer = new ScrollView();
            categoryContainer.style.flexGrow = 0;
            categoryContainer.style.flexShrink = 1;
            categoryContainer.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
            categoryContainer.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);
            
            categoryContainer.style.paddingBottom = 5;
            categoryContainer.style.paddingLeft = 10;
            categoryContainer.style.paddingRight = 10;
            
            categoryContainer.contentContainer.style.flexGrow = 1;
            categoryContainer.contentContainer.style.height = new StyleLength(StyleKeyword.Auto);
            categoryContainer.contentContainer.style.width = new StyleLength(StyleKeyword.Auto);
            categoryContainer.name = "categoryContainer";
            categoryContainer.contentContainer.name = "categoryContentContainer";
            
            Label label = new Label(RemoveUntilFirstPoint(names[i].Replace("MarketSystem.", string.Empty)).Replace("Item",string.Empty));
            label.style.maxWidth = new StyleLength(StyleKeyword.Auto);
            label.style.fontSize = 20;
            label.style.paddingBottom = 10;
            label.style.paddingLeft = 10;
            
            VisualElement contentContainer = new VisualElement();
            contentContainer.style.borderBottomColor = new StyleColor(new Color(.2f,.2f,.2f));
            contentContainer.style.borderBottomWidth = new StyleFloat(1);
            contentContainer.style.paddingBottom = new StyleLength(10);
            contentContainer.style.alignContent = new StyleEnum<Align>(Align.FlexStart);
            contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            contentContainer.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);
            
            categoryContainer.Add(label);
            categoryContainer.Add(contentContainer);
            
            if (itemsPerClass.ContainsKey(names[i]))
            {
                foreach (BaseItem baseItem in itemsPerClass[names[i]])
                    contentContainer.Add(CreateItemSelectionButton(baseItem));
            }

            Type currentType = types[i];

            Button addButton = new Button {text = "+"};
            addButton.clicked += () => CreateNewItem(currentType);
            addButton.style.alignSelf = new StyleEnum<Align>(Align.Center);
            addButton.AddToClassList("AddButton");
            contentContainer.Add(addButton);
            lower.contentContainer.Add(categoryContainer);
        }
    }

    private VisualElement CreateItemSelectionButton(BaseItem baseItem)
    {
        VisualElement visualElement = new VisualElement();
        Button button = new Button();
        button.style.height = 50;
        button.style.width = 50;
        button.text = string.Empty;

        Sprite icon = baseItem.icon;
        button.style.backgroundImage = new StyleBackground(icon?icon.texture:null);
        button.style.alignSelf = new StyleEnum<Align>(Align.Center);
        Label label = new Label {text = baseItem.name};
        label.style.alignSelf = new StyleEnum<Align>(Align.Center);

        visualElement.Add(button);
        visualElement.Add(label);
        button.clicked += ()=> SelectItem(baseItem);
        
        Manipulator manipulator = new ContextualMenuManipulator(x=> MenuBuilder (x, baseItem)){ target = button};
        button.AddManipulator(manipulator);
        
        return visualElement;
    }

    private void MenuBuilder(ContextualMenuPopulateEvent obj, BaseItem baseItem) => obj.menu.AppendAction("Delete",x => DestroyItem(baseItem));

    private void CreateNewItem(Type type)
    {
        AssetDatabase.Refresh();
        Object uObject = CreateInstance(type);
        MarketSystem.MarketManager.Instance.AddNewItem(uObject as BaseItem);
        BaseItem nItem = uObject as BaseItem;
        
        int i = -1;
        string assetName;
        do
        {
            i++;
            assetName = $"new {type.Name} {i}";
        } while (MarketManager.DoesItemExists(assetName));

        nItem.name = assetName;

        string filePath = MarketManager.CreateFoldersForAsset("Items", nItem);

        AssetDatabase.CreateAsset(nItem,filePath + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Render();
    }

    

    private void OnAllCategorySelected(List<string> allCategories)
    {
        if (selectedItemCategories.Count != allCategories.Count)
        {
            selectedItemCategories.Clear();
            selectedItemCategories.AddRange(allCategories);
        }
        else
            selectedItemCategories.Clear();
        Render();
    }
    
     private void OnItemCategorySelected(bool toggleValue, string categoryName)
    {
        if (toggleValue && !selectedItemCategories.Contains(categoryName))
            selectedItemCategories.Add(categoryName);
        else if(!toggleValue && selectedItemCategories.Contains(categoryName))
            selectedItemCategories.Remove(categoryName);
        Render();
    }
     
     
    #endregion

    
    private void SelectItem(BaseItem baseItem)
    {
        selectedItem = baseItem;
        Render();
    }

    private VisualElement CreateSelectedItemView<T>(T item) where T : BaseItem
    {
        VisualElement container = new VisualElement();
        
        VisualElement objectNameContainer = new VisualElement();
        objectNameContainer.style.paddingBottom = 10;
        objectNameContainer.style.paddingTop = 10;
        objectNameContainer.style.paddingLeft = 10;
        objectNameContainer.style.paddingRight = 10;
        
        objectNameContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        Label fileNameLabel = new Label(){ text = "File Name:"};
        TextField objectFieldName = new TextField() { value = item.name};
        objectFieldName.style.flexGrow = 1;
        objectFieldName.style.flexShrink = 0;
        
        Button deleteButton = new Button(){text = "X"};
        deleteButton.clicked += () => DestroyItem(item);
        deleteButton.AddToClassList("DangerButton");
        deleteButton.style.flexShrink = 0;
        Image image = new Image();
        if(item.icon != null)
            image.image = item.icon.texture;
        
        image.style.backgroundColor = new StyleColor(new Color(.2f,.2f,.2f));
        image.style.width = 100;
        image.style.height = 100;
        image.style.marginBottom = 10;
        image.style.alignSelf = new StyleEnum<Align>(Align.Center);

        T clone = Instantiate(item);
        clone.name = item.name;
        IMGUIContainer imguiContainer = new IMGUIContainer();
        Editor editor = UnityEditor.Editor.CreateEditor(clone);
        imguiContainer.onGUIHandler = () => editor.OnInspectorGUI();
        
        VisualElement buttonContainer = new VisualElement();
        buttonContainer.style.alignSelf = new StyleEnum<Align>(Align.Center);
        buttonContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        
        Button saveButton = new Button(){text = "Save Changes"};

        void OnSaveButtonOnclicked()
        {
            EditorUtility.CopySerialized(clone, item);
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), objectFieldName.text);
            Render();
        }

        saveButton.clicked += OnSaveButtonOnclicked;
        saveButton.AddToClassList("SaveButton");
        saveButton.style.width = 100;
        saveButton.style.height = 25;
        
        Button cancelButton = new Button(){text = "Cancel"};
        cancelButton.clicked += Render;
        cancelButton.AddToClassList("DangerButton");
        cancelButton.style.width = 70;
        cancelButton.style.height = 25;
        
        objectNameContainer.Add(fileNameLabel);
        objectNameContainer.Add(objectFieldName);
        objectNameContainer.Add(deleteButton);
        
        
        buttonContainer.Add(saveButton);
        buttonContainer.Add(cancelButton);
        buttonContainer.style.marginBottom = 10;
        buttonContainer.style.marginTop = 10;
        
        container.Add(objectNameContainer);
        container.Add(image);
        container.Add(imguiContainer);
        container.Add(buttonContainer);
        
        return container;
    }

    private void DestroyProduct<T>(T product) where T : BaseProduct
    {
           if (!EditorUtility.DisplayDialog("Are you sure?", $"Are you sure you want to permanently delete {product}?",
            "Delete", "Cancel")) return;
        MarketManager.RemoveProduct(product);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(product));
        Render();
    }

    private void DestroyItem<T>(T item) where T : BaseItem
    {
        if (!EditorUtility.DisplayDialog("Are you sure?", $"Are you sure you want to permanently delete {item}?",
            "Delete", "Cancel")) return;
        MarketManager.RemoveItem(item);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
        Render();
    }

    private VisualElement CreateSelectedProductView(ScriptableObject baseItem)
    {
        VisualElement container = new VisualElement();
        container.style.flexGrow = 1;
        Label label = new Label("Selection");
        label.style.fontSize = 25;
        label.style.alignSelf = new StyleEnum<Align>(Align.Auto);
        container.Add(label);
        
        if (selectedProduct != null)
        {
            IMGUIContainer imguiContainer = new IMGUIContainer();
            Editor editor = UnityEditor.Editor.CreateEditor(baseItem);
            imguiContainer.onGUIHandler = () => editor.OnInspectorGUI();
            imguiContainer.style.alignSelf = new StyleEnum<Align>(Align.Auto);
            container.Add(imguiContainer);
        }

        return container;
    }

    
    private static IEnumerable<Type> GetAllSubclassTypes<T>(bool keepAbstracts) 
    {
        return from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where (type.IsSubclassOf(typeof(T)) && (keepAbstracts || !type.IsAbstract))
            select type;
    }

    private VisualElement CreateProductSelectionButton(BaseProduct baseProduct)
    {
        VisualElement visualElement = new VisualElement();
        Button button = new Button();
        button.style.height = 50;
        button.style.width = 50;
        button.text = string.Empty;

        Sprite icon = baseProduct.Icon;
        button.style.backgroundImage = new StyleBackground(icon?icon.texture:null);
        Label label = new Label {text = RemoveUntilFirstPoint(baseProduct.name)};
        
        visualElement.Add(button);
        visualElement.Add(label);
        button.clicked += ()=> ProductButtonPressed(baseProduct);
        
        Manipulator manipulator = new ContextualMenuManipulator(x=> ProductMenuBuilder (x, baseProduct)){ target = button};
        button.AddManipulator(manipulator);
    
        return visualElement;
    }

    private void ProductMenuBuilder(ContextualMenuPopulateEvent obj, BaseProduct baseItem) => obj.menu.AppendAction("Delete",x => DestroyProduct(baseItem));

    private VisualElement CreateSelectCatalogButton(BaseCatalog catalog)
    {
        Button button = new Button();
        button.text = catalog.name;
        button.clicked += () => SelectCatalog(catalog);
        return button;
    }

    private void SelectCatalog(BaseCatalog catalog)
    {
        selectedCatalog = catalog;
        Render();
    }

    private void ProductButtonPressed(BaseProduct baseProduct)
    {
        selectedProduct = baseProduct;
        Render();
    }
    
    private static ListView CreateListView(List<string> items, List<string> activeItems, Action<bool, string> onToggleCallback, string removeFromName = null)
    {
        ListView listView = new ListView();
        listView.style.width = StyleKeyword.Auto;
        listView.style.flexShrink = 0;
        listView.style.flexGrow = 1;
        listView.style.height = new StyleLength(StyleKeyword.Auto);
        listView.contentContainer.style.flexBasis = new StyleLength(StyleKeyword.Auto);
     
        for (int i = 0; i < items.Count; i++)
        {
            string itemName = items[i];
            itemName = RemoveUntilFirstPoint(itemName);
            if (removeFromName != null) itemName = itemName.Replace(removeFromName, string.Empty);

            Toggle toggle = new Toggle {text = itemName};
            int index = i;
            toggle.SetValueWithoutNotify(activeItems.Contains(items[index]));
            string value = items[index];
            toggle.RegisterValueChangedCallback(x => onToggleCallback.Invoke(x.newValue, value));
            listView.contentContainer.Add(toggle);
        }
        return listView;
    }

    private static string RemoveUntilFirstPoint(string itemName)
    {
        int pointIndex = itemName.IndexOf('.');
        if (pointIndex >= 0)
            itemName = itemName.Remove(0, pointIndex+1);
        return itemName;
    }

    private VisualElement RenderSelectedItem()
    {
        VisualElement visualElement = new VisualElement();
        
        Label selectedLabel = new Label(){text = "Selection"};
        selectedLabel.style.fontSize = 20;
        visualElement.Add(selectedLabel);
        if (selectedItem != null)
            visualElement.Add(CreateSelectedItemView(selectedItem));
        return visualElement;
    }

}