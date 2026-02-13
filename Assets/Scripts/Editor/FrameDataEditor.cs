using SkillIssue.Animations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FrameDataEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    
    private ObjectField animationdataField;
    private ListView frameEventList;
    private VisualElement frameInspector;

    private PropertyField propertyField;

    private Button addFrameEventButton;
    private Button removeFrameEventButton;

    private AnimationData currentAnimationData;
    private FrameEvent currentFrameEvent;

    int selecteFrameIndex = 0;

    [MenuItem("Window/UI Toolkit/FrameDataEditor")]
    public static void ShowExample()
    {
        FrameDataEditor wnd = GetWindow<FrameDataEditor>();
        wnd.titleContent = new GUIContent("FrameDataEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement treeAsset = m_VisualTreeAsset.Instantiate();
        root.Add(treeAsset);

        animationdataField = treeAsset.Q<ObjectField>("AnimationDataField");
        frameEventList = treeAsset.Q<ListView>("FrameEventList");
        addFrameEventButton = treeAsset.Q<Button>("AddFrameEventButton");
        removeFrameEventButton = treeAsset.Q<Button>("RemoveFrameEventButton");
        frameInspector = treeAsset.Q<VisualElement>("FrameEventInspector");

        propertyField = treeAsset.Q<PropertyField>("FrameEventField");

        addFrameEventButton.RegisterCallback<MouseUpEvent>((evt) => AddFrameEvent());
        removeFrameEventButton.RegisterCallback<MouseUpEvent>((evt) => RemoveFrameEvent());

        animationdataField.RegisterValueChangedCallback(OnAnimationDataChanged);
    }

    void OnAnimationDataChanged(ChangeEvent<Object> evt)
    {

        if (evt.newValue == null)
        {
            Debug.Log("value is null");
            return;
        }
        currentAnimationData = evt.newValue as AnimationData;
        PopulateList();

    }

    void PopulateList()
    {
        frameEventList.Clear();

        frameEventList.itemsSource = currentAnimationData.FrameEvents();

        frameEventList.makeItem = () => new Label();
        frameEventList.bindItem = (element, index) =>
        {
            (element as Label).text = currentAnimationData.FrameEvents()[index].Frame.ToString();
        };

        frameEventList.selectionType = SelectionType.Single;
        frameEventList.selectionChanged += _ =>
        {
            selecteFrameIndex = frameEventList.selectedIndex;
            currentFrameEvent = currentAnimationData.FrameEvents()[selecteFrameIndex];
            UpdateFrameInspector();
        };
    }

    void AddFrameEvent()
    {
        if (currentAnimationData == null)
            return;
        currentAnimationData.FrameEvents().Add(new FrameEvent());
        frameEventList.Rebuild();
    }

    void RemoveFrameEvent()
    {
        if (currentAnimationData == null || currentAnimationData.FrameEvents().Count == 0)
            return;
        currentAnimationData.FrameEvents().RemoveAt(selecteFrameIndex);
        frameEventList.Rebuild();
    }   
    
    void UpdateFrameInspector()
    {
        // Inspector Method

        //frameInspector.Clear();
        //InspectorElement propertyField = new InspectorElement();
        SerializedObject serializedObject = new SerializedObject(currentAnimationData);

        //propertyField.Bind(serializedObject);

        //frameInspector.Add(propertyField);
        SerializedProperty listProperty = serializedObject.FindProperty("frameEvents");
        SerializedProperty selectedFrame = listProperty.GetArrayElementAtIndex(selecteFrameIndex) as SerializedProperty;
        propertyField.BindProperty(selectedFrame);
        
    }
}

