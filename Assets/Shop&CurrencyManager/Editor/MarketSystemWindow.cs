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

    private BaseItem _selectedItem;
    private BaseCatalog _selectedCatalog;    
    private BaseProduct _selectedProduct;
    private BaseProduct _selectedPreviewProduct;

    private List<string> selectedItemCategories = new List<string>();
    private List<string> selectedProductCategories = new List<string>();

    private enum States{Items,Catalogs,Products}

    private States currentState;
    
    [MenuItem("Window/Ishimine/MarketManager %#k", priority = 1)]
    public static void ShowWindow()
    {
        MarketSystemWindow wnd = GetWindow<MarketSystemWindow>();
        wnd.titleContent = new GUIContent("MarketManager");
        wnd.ScanForAssets();
    }

    private VisualTreeAsset catalogsVisualTree;
    private VisualTreeAsset itemsVisualTree;
    private VisualTreeAsset productsVisualTree;
    private VisualTreeAsset scriptableObjectEdit_View;
    
    public void OnEnable()
    {
        VisualElement root = rootVisualElement;
        var visualTree = Resources.Load<VisualTreeAsset>("MarketManager_Main");
        visualTree.CloneTree(root);
        var styleSheet = Resources.Load<StyleSheet>("MarketManager_Style");
        root.styleSheets.Add(styleSheet);

        catalogsVisualTree  = Resources.Load<VisualTreeAsset>("Catalogs_View");
        itemsVisualTree  = Resources.Load<VisualTreeAsset>("Items_View");
        productsVisualTree  = Resources.Load<VisualTreeAsset>("Products_View");
        scriptableObjectEdit_View  = Resources.Load<VisualTreeAsset>("ScriptableObjectEdit_View");
        
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
        body.Clear();
        catalogsVisualTree.CloneTree(body);

        ListView listView = body.Q<ListView>("CatalogsList");

        List<BaseCatalog> catalogs = new List<BaseCatalog>(MarketManager.catalogs);
        catalogs.Sort(NameComparer);
        for (int i = 0; i < catalogs.Count; i++)
        {
            Button button = new Button {text = catalogs[i].name};
            int index = i;
            button.clicked += () => OnCatalogSelected(catalogs[index]);
            
            button.AddManipulator( 
                new ContextualMenuManipulator( 
                     x => 
                         AddContextMenuOptions(
                             x.menu, 
                             new []
                             {
                                 new Tuple<string, Action>("ChangeID", () => ChangeCatalogName(catalogs[index])),
                                 new Tuple<string, Action>("Delete", () => DestroyCatalog(catalogs[index])),
                             })));
            listView.Add(button);
        }

        Button newCatalogButton = new Button {text = "+"};
        newCatalogButton.clicked += CreateNewCatalogRequest;
        newCatalogButton.style.width = 25;
        newCatalogButton.style.height = 25;
        newCatalogButton.style.alignSelf = new StyleEnum<Align>(Align.Center);
        listView.Add(newCatalogButton);

        ListView catalogContent = body.Q<ListView>("LeftContent");
        catalogContent.style.flexGrow = 1;
        catalogContent.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        catalogContent.contentContainer.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);
        
        Label label = body.Q<Label>("CurrentCatalogLabel");

        if (_selectedCatalog != null)
        {
            label.text = _selectedCatalog.name;
            List<BaseProduct> baseProducts = new List<BaseProduct>(_selectedCatalog.Products);
            baseProducts.Sort(NameComparer);
            for (int i = 0; i < baseProducts.Count; i++)
            {
                int localIndex = i;
                VisualElement prodSelection = CreateProductSelectionButton(
                    baseProducts[i], 
                    baseProduct => RemoveFrom(baseProduct, _selectedCatalog),
                    new []
                    {
                        new Tuple<string, Action>("Preview", ()=> PreviewProduct(baseProducts[localIndex])),
                        new Tuple<string, Action>("Edit", ()=> GoToProductEdit(baseProducts[localIndex])),
                        new Tuple<string, Action>("Delete", ()=> DestroyProduct(baseProducts[localIndex]))
                    });
                catalogContent.Add(prodSelection);
            }
        }   
        else
            label.text = "-";

        #region SelectionList

        GetProductNamesAndTypes(out List<Type> types, out List<string> names);
        ListView typeSelectionList = body.Q<ListView>("TypesList");

        Button allButton = new Button {text = "Select All"};
        allButton.style.width = 70;
        allButton.clicked += () => OnAllCategorySelected(selectedProductCategories, names);

        FillListWithToggle(names, "Product", selectedProductCategories, typeSelectionList, OnProductTypeSelected);
        typeSelectionList.contentContainer.Insert(0, allButton);
        typeSelectionList.Insert(0, allButton);

        #endregion

        #region Preview
        if (_selectedPreviewProduct != null)
        {
            VisualElement selectionRender = body.Q<VisualElement>("Right");

            selectionRender.style.paddingLeft = 5;
            selectionRender.style.paddingRight = 5;
            selectionRender.style.paddingTop = 5;
            selectionRender.style.paddingBottom = 5;
            
            Button closeButton = new Button() {text = "X"};
            closeButton.clicked += () =>
            {
                _selectedPreviewProduct = null;
                Render();
            };
            closeButton.style.width = 20;
            closeButton.style.height = 20;

            scriptableObjectEdit_View.CloneTree(selectionRender);
            Image icon = new Image();
            IMGUIContainer soRender = selectionRender.Q<IMGUIContainer>("Render");
            icon.style.alignSelf = new StyleEnum<Align>(Align.Center);
            icon.style.height = 100;
            icon.style.width = 100;
            icon.style.backgroundColor = new Color(.25f, .25f, .25f);
            icon.style.marginTop = 10;
            icon.style.marginBottom = 10;
            
            selectionRender.Insert(0,icon);
            selectionRender.Insert(0,closeButton);
            
            if (_selectedPreviewProduct != null)
            {
                Editor editor = Editor.CreateEditor(_selectedPreviewProduct);
                soRender.onGUIHandler = () => editor.OnInspectorGUI();
                if (_selectedPreviewProduct.Icon != null) icon.image = _selectedPreviewProduct.Icon.texture;
            }

            VisualElement buttons = body.Q<VisualElement>("Buttons");
            if (buttons != null)
            {
                buttons.style.opacity = 0;
                buttons.SetEnabled(false);
            }
            soRender.SetEnabled(false);
        }
        #endregion

        #region Add Selection
        VisualElement lowerContent = body.Q<VisualElement>("LowerContent");
        lowerContent.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        lowerContent.contentContainer.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);

        HashSet<BaseProduct> products = new HashSet<BaseProduct>();
        Dictionary<string, List<BaseProduct>> productsByClass = MarketManager.GetProductsByClass();

        if(_selectedCatalog == null) return;
        
        foreach (KeyValuePair<string,List<BaseProduct>> keyValuePair in productsByClass)
        {
            if(!selectedProductCategories.Contains(keyValuePair.Key)) continue;
            for (int i = 0; i < keyValuePair.Value.Count; i++)
                if(!_selectedCatalog.Products.Contains(keyValuePair.Value[i]))
                    products.Add(keyValuePair.Value[i]);
        }

        foreach (BaseProduct itemProduct in products)
            lowerContent.Add(
                CreateProductSelectionButton(
                    itemProduct, 
                    baseProduct => AddTo(itemProduct, _selectedCatalog),
                    new []
                    {
                        new Tuple<string, Action>("Preview", ()=> PreviewProduct(itemProduct)),
                        new Tuple<string, Action>("Edit", ()=> GoToProductEdit(itemProduct)),
                        new Tuple<string, Action>("Delete", ()=> DestroyProduct(itemProduct))
                    }));
        
        #endregion
    }

    private void GoToProductEdit(BaseProduct itemProduct)
    {
        _selectedProduct = itemProduct;
        ChangeTab(States.Products);
    }

    private void AddTo(BaseProduct baseProduct, BaseCatalog selectedCatalog)
    {
        _selectedPreviewProduct = null;
        if (!selectedCatalog.Products.Contains(baseProduct))
            selectedCatalog.Products.Add(baseProduct);
        Render();
    }

    private void RemoveFrom(BaseProduct baseProduct, BaseCatalog selectedCatalog)
    {
        _selectedPreviewProduct = null;
        Debug.Log($"Removing {baseProduct} to {selectedCatalog}");
        if (selectedCatalog.Products.Contains(baseProduct))
            selectedCatalog.Products.Remove(baseProduct);
        Render();
    }

    private void CreateNewCatalogRequest()
    {
        AssetDatabase.Refresh();
        BaseCatalog nCatalog = CreateInstance<BaseCatalog>();

        int i = -1;
        string assetName;
        do
        {
            i++;
            assetName = string.Format(nCatalog.DefaultNaming, i);
        } while (MarketManager.DoesCatalogIDExists(assetName));

        nCatalog.name = assetName;
        MarketSystem.MarketManager.Instance.AddNewCatalog(nCatalog);

        string filePath = MarketManager.CreateFoldersForAsset("Catalogs", nCatalog);

        AssetDatabase.CreateAsset(nCatalog, filePath + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Render();
    }


    private void OnCatalogSelected(BaseCatalog catalog)
    {
        _selectedCatalog = catalog;
        Render();
    }

    private static int NameComparer(Object x, Object y) => string.Compare(x.name, y.name, StringComparison.Ordinal);

    #endregion

    #region Products Panel

    private void RenderProductsPanel()
    {
        VisualElement body = rootVisualElement.Q<VisualElement>("body");
        body.Clear();
        productsVisualTree.CloneTree(body);
        
        GetProductNamesAndTypes(out List<Type> types, out List<string> names);

        #region SelectionList
        ListView typeSelectionList = rootVisualElement.Q<ListView>("TypesList");
        
        Button allButton = new Button {text = "Select All"};
        allButton.style.width = 70;
        allButton.clicked += () => OnAllCategorySelected(selectedProductCategories, names);
        
        FillListWithToggle(names, "Product",selectedProductCategories , typeSelectionList, OnProductTypeSelected);
        typeSelectionList.contentContainer.Insert(0, allButton);
        typeSelectionList.Insert(0, allButton);
        #endregion
        
        #region Selection Icons
        ScrollView categoryContainer = body.Q<ScrollView>("Lower");

        Dictionary<string, List<BaseProduct>> itemsPerClass = MarketManager.GetProductsByClass();
        Label categoriesLabel = new Label() {text = "TYPES"};
        categoriesLabel.style.fontSize = 20;
        for (int i = 0; i < names.Count; i++)
        {
            if (!selectedProductCategories.Contains(names[i])) continue;

            Label label = new Label(RemoveUntilFirstPoint(names[i].Replace("MarketSystem.", string.Empty))
                .Replace("Product", string.Empty));
            label.style.maxWidth = new StyleLength(StyleKeyword.Auto);
            label.style.fontSize = 20;
            label.style.paddingBottom = 10;
            label.style.paddingLeft = 10;

            VisualElement contentContainer = new VisualElement();
            contentContainer.style.borderBottomColor = new StyleColor(new Color(.2f, .2f, .2f));
            contentContainer.style.borderBottomWidth = new StyleFloat(1);
            contentContainer.style.paddingBottom = new StyleLength(10);
            contentContainer.style.alignContent = new StyleEnum<Align>(Align.FlexStart);
            contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            contentContainer.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);
            contentContainer.style.maxWidth = 600;

            if (itemsPerClass.ContainsKey(names[i]))
            {
                foreach (BaseProduct baseProduct in itemsPerClass[names[i]])
                    contentContainer.Add(
                        CreateProductSelectionButton(
                            baseProduct, 
                            ProductSelected,
                            new[]
                            {
                                new Tuple<string, Action>("ChangeID",()=> ChangeProductName(baseProduct)),
                                new Tuple<string, Action>("Delete",()=> DestroyProduct(baseProduct))
                            }));
            }

            Type currentType = types[i];

            Button addButton = new Button {text = "+"};
            addButton.clicked += () => CreateNewProduct(currentType);
            addButton.style.alignSelf = new StyleEnum<Align>(Align.Center);
            addButton.AddToClassList("AddButton");
            contentContainer.Add(addButton);

            categoryContainer.Add(label);
            categoryContainer.Add(contentContainer);
        }
        #endregion
        
        #region Selection
        VisualElement selectionRender = body.Q<VisualElement>("SelectionContainer");
        scriptableObjectEdit_View.CloneTree(selectionRender);

        Image icon = new Image();

        IMGUIContainer soRender = selectionRender.Q<IMGUIContainer>("Render");
        icon.style.alignSelf = new StyleEnum<Align>(Align.Center);
        icon.style.height = 100;
        icon.style.width = 100;
        icon.style.backgroundColor = new Color(.25f, .25f, .25f);
        icon.style.marginTop = 10;
        icon.style.marginBottom = 10;
        selectionRender.Insert(1, icon);

        BaseProduct selection = _selectedProduct;

        if (selection != null)
        {
            BaseProduct clone = Instantiate(selection);
            clone.name = selection.name;
            
            Editor editor = Editor.CreateEditor(clone);
            soRender.onGUIHandler = () => editor.OnInspectorGUI();
            if (selection.Icon != null) icon.image = selection.Icon.texture;

            Button deleteButton = body.Q<Button>("DeleteButton");
            deleteButton.clicked += () => DestroyProduct(selection);

            Button saveButton = body.Q<Button>("SaveButton");
            saveButton.clicked += () => SaveScriptableObject(clone, selection);

            Button cancelButton = body.Q<Button>("CancelButton");
            cancelButton.clicked += Render;

            Button changeIdButton = body.Q<Button>("ChangeIdButton");
            changeIdButton.clicked += () => ChangeProductName(selection);
        }
    }

    private void PreviewProduct(BaseProduct baseProduct)
    {
        _selectedPreviewProduct = baseProduct;
        Render();
    }

    private static void GetProductNamesAndTypes(out List<Type> types, out List<string> names)
    {
        types = new List<Type>(GetAllSubclassTypes<BaseProduct>(false));
        names = new List<string>();
        for (int i = 0; i < types.Count; i++) names.Add(types[i].ToString());
    }

    #endregion

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
            assetName = string.Format(nItem.DefaultNaming, i);
        } while (MarketManager.DoesProductIDExists(assetName));
        
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
        itemsVisualTree.CloneTree(body);
        
        List<Type> types = new List<Type>(GetAllSubclassTypes<BaseItem>(false));
        List<string> names = new List<string>();
        for (int i = 0; i < types.Count; i++) names.Add(types[i].ToString());
                
        #region SelectionList
        ListView typeSelectionList = rootVisualElement.Q<ListView>("TypesList");
        Button allButton = new Button {text = "Select All"};
        allButton.style.width = 70;
        allButton.clicked += () => OnAllCategorySelected(selectedItemCategories, names);
        
        FillListWithToggle(names, "Item",selectedItemCategories, typeSelectionList, OnItemCategorySelected);
        
        typeSelectionList.contentContainer.Insert(0, allButton);
        typeSelectionList.Insert(0, allButton);
        #endregion

        #region Selection Icons
        ScrollView categoryContainer = body.Q<ScrollView>("Lower");

        Dictionary<string, List<BaseItem>> itemsPerClass = MarketManager.GetItemsByClass();
        Label categoriesLabel = new Label() {text = "ITEM TYPES"};
        categoriesLabel.style.fontSize = 20;
        for (int i = 0; i < names.Count; i++)
        {
            if (!selectedItemCategories.Contains(names[i])) continue;

            Label label = new Label(RemoveUntilFirstPoint(names[i].Replace("MarketSystem.", string.Empty))
                .Replace("Item", string.Empty));
            label.style.maxWidth = new StyleLength(StyleKeyword.Auto);
            label.style.fontSize = 20;
            label.style.paddingBottom = 10;
            label.style.paddingLeft = 10;

            VisualElement contentContainer = new VisualElement();
            contentContainer.style.borderBottomColor = new StyleColor(new Color(.2f, .2f, .2f));
            contentContainer.style.borderBottomWidth = new StyleFloat(1);
            contentContainer.style.paddingBottom = new StyleLength(10);
            contentContainer.style.alignContent = new StyleEnum<Align>(Align.FlexStart);
            contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            contentContainer.style.flexWrap = new StyleEnum<Wrap>(Wrap.Wrap);
            contentContainer.style.maxWidth = 600;

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

            categoryContainer.Add(label);
            categoryContainer.Add(contentContainer);
        }
        #endregion

        #region Selection
        VisualElement selectionRender = body.Q<VisualElement>("SelectionContainer");
        scriptableObjectEdit_View.CloneTree(selectionRender);

        Image icon = new Image();

        IMGUIContainer soRender = selectionRender.Q<IMGUIContainer>("Render");
        icon.style.alignSelf = new StyleEnum<Align>(Align.Center);
        icon.style.height = 100;
        icon.style.width = 100;
        icon.style.backgroundColor = new Color(.25f, .25f, .25f);
        icon.style.marginTop = 10;
        icon.style.marginBottom = 10;
        selectionRender.Insert(1, icon);

        BaseItem selectedItem = _selectedItem;

        if (selectedItem != null)
        {
            BaseItem clone = Instantiate(selectedItem);
            clone.name = selectedItem.name;
            
            Editor editor = Editor.CreateEditor(clone);
            soRender.onGUIHandler = () => editor.OnInspectorGUI();
            if (selectedItem.icon != null) icon.image = selectedItem.icon.texture;

            Button deleteButton = body.Q<Button>("DeleteButton");
            deleteButton.clicked += () => DestroyItem(selectedItem);

            Button saveButton = body.Q<Button>("SaveButton");
            saveButton.clicked += () => SaveScriptableObject(clone, selectedItem);

            Button cancelButton = body.Q<Button>("CancelButton");
            cancelButton.clicked += Render;

            Button changeIdButton = body.Q<Button>("ChangeIdButton");
            changeIdButton.clicked += () => ChangeItemName(selectedItem);
        }
        #endregion
    }

    private void ChangeItemName(BaseItem selection) => ChangeTabNameWindow.Show(selection, nName => !MarketManager.DoesItemIDExists(nName), Render);
    private void ChangeProductName(BaseProduct selection) => ChangeTabNameWindow.Show(selection, nName => !MarketManager.DoesProductIDExists(nName), Render);
    private void ChangeCatalogName(BaseCatalog selection) => ChangeTabNameWindow.Show(selection, nName => !MarketManager.DoesCatalogIDExists(nName), Render);

    private void SaveScriptableObject<T>(T clone, T selectedItem) where T : ScriptableObject
    {
        EditorUtility.CopySerialized(clone, selectedItem);
        Render();
    }

    private void FillListWithToggle(List<string> names, string removeFromName, List<string> sourceList, ListView listView, Action<bool,string> OnSelected)
    {
        for (int i = 0; i < names.Count; i++)
        {
            string itemName = names[i];
            itemName = RemoveUntilFirstPoint(itemName);
            itemName = itemName.Replace(removeFromName, string.Empty);

            Toggle toggle = new Toggle {text = itemName};
            int index = i;
            toggle.SetValueWithoutNotify(sourceList.Contains(names[index]));
            string value = names[index];
            toggle.RegisterValueChangedCallback(x => OnSelected.Invoke(x.newValue, value));
            listView.contentContainer.Add(toggle);
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
        
        Manipulator manipulator = new ContextualMenuManipulator(
                contextualMenuPopulateEvent => 
                    AddContextMenuOptions(
                        contextualMenuPopulateEvent,
                        new []
                        {
                            new Tuple<string, Action>("ChangeID", ()=>ChangeItemName(baseItem)),
                            new Tuple<string, Action>("Delete", ()=>DestroyItem(baseItem))
                        }
                        )) {target = button};
        
        button.AddManipulator(manipulator);
        return visualElement;
    }

    private void AddContextMenuOptions(ContextualMenuPopulateEvent contextualMenuPopulateEvent,
        Tuple<string, Action>[] contextualMenuOptions) =>
        AddContextMenuOptions(contextualMenuPopulateEvent.menu, contextualMenuOptions);

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
            assetName = string.Format(nItem.DefaultNaming, i);
        } while (MarketManager.DoesItemIDExists(assetName));

        nItem.name = assetName;

        string filePath = MarketManager.CreateFoldersForAsset("Items", nItem);

        AssetDatabase.CreateAsset(nItem,filePath + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Render();
    }
    
    private void OnAllCategorySelected(List<string> selectedCategories, List<string> allCategories)
    {
        if (selectedCategories.Count != allCategories.Count)
        {
            selectedCategories.Clear();
            selectedCategories.AddRange(allCategories);
        }
        else
            selectedCategories.Clear();
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
        _selectedItem = baseItem;
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
        
        return container;
    }

    
    
    private void DestroyCatalog(BaseCatalog catalog)
    {    
        if (!EditorUtility.DisplayDialog("Are you sure?", $"Are you sure you want to permanently delete {catalog}?",
            "Delete", "Cancel")) return;
        
        MarketManager.RemoveCatalog(catalog);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(catalog));
        Render();
    }

    
    
    private void DestroyProduct(BaseProduct item)
    {
        if (!EditorUtility.DisplayDialog("Are you sure?", $"Are you sure you want to permanently delete {item}?",
            "Delete", "Cancel")) return;
        
        MarketManager.RemoveProduct(item);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
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

    private static IEnumerable<Type> GetAllSubclassTypes<T>(bool keepAbstracts) 
    {
        return from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where (type.IsSubclassOf(typeof(T)) && (keepAbstracts || !type.IsAbstract))
            select type;
    }

    private VisualElement CreateProductSelectionButton(BaseProduct baseProduct, Action<BaseProduct> OnProductSelected, Tuple<string,Action>[] contextualMenuOptions)
    {
        VisualElement visualElement = new VisualElement();
        Button button = new Button();
        button.style.height = 50;
        button.style.width = 50;
        button.text = string.Empty;
        button.style.alignSelf = new StyleEnum<Align>(Align.Center);
        Sprite icon = baseProduct.Icon;
        button.style.backgroundImage = new StyleBackground(icon?icon.texture:null);
        Label label = new Label {text = RemoveUntilFirstPoint(baseProduct.name)};
        label.style.alignSelf = new StyleEnum<Align>(Align.Center);
        button.style.alignSelf = new StyleEnum<Align>(Align.Center);
        
        visualElement.Add(button);
        visualElement.Add(label);
        button.clicked += ()=> OnProductSelected.Invoke(baseProduct);

        if (contextualMenuOptions == null) return visualElement;
        
        
        Manipulator manipulator = new ContextualMenuManipulator(contextualMenuPopulateEvent => AddContextMenuOptions(contextualMenuPopulateEvent.menu, contextualMenuOptions)) {target = button};
        button.AddManipulator(manipulator);
        
        return visualElement;
    }

    private void AddContextMenuOptions(DropdownMenu objMenu, Tuple<string, Action>[] contextualMenuOptions)
    {
        for (int i = 0; i < contextualMenuOptions.Length; i++)
        {
            int localIndex = i;
            objMenu.AppendAction(contextualMenuOptions[i].Item1, dropdownMenuAction => contextualMenuOptions[localIndex].Item2.Invoke());
        }
    }

    private void SelectCatalog(BaseCatalog catalog)
    {
        _selectedCatalog = catalog;
        Render();
    }

    private void ProductSelected(BaseProduct baseProduct)
    {
        _selectedProduct = baseProduct;
        Render();
    }

    private static string RemoveUntilFirstPoint(string itemName)
    {
        int pointIndex = itemName.IndexOf('.');
        if (pointIndex >= 0)
            itemName = itemName.Remove(0, pointIndex+1);
        return itemName;
    }


}