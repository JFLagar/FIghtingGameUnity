using SkillIssue;
using SkillIssue.Animations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FrameDataEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private ObjectField animationdataField;
    private ObjectField modelField;
    private ListView frameEventList;
    private VisualElement frameInspector;
    private VisualElement buttonContainer;

    private PropertyField propertyField;

    private Button addFrameEventButton;
    private Button removeFrameEventButton;

    private AnimationData currentAnimationData;
    private FrameEvent currentFrameEvent;

    int selectedFrameIndex = 0;

    private AnimationWindow animationWindow;

    GameObject model;

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
        modelField = treeAsset.Q<ObjectField>("PrefabModelField");
        frameEventList = treeAsset.Q<ListView>("FrameEventList");
        addFrameEventButton = treeAsset.Q<Button>("AddFrameEventButton");
        removeFrameEventButton = treeAsset.Q<Button>("RemoveFrameEventButton");
        frameInspector = treeAsset.Q<VisualElement>("FrameEventInspector");
        buttonContainer = treeAsset.Q<VisualElement>("ButtonContainer");

        propertyField = treeAsset.Q<PropertyField>("FrameEventField");

        addFrameEventButton.RegisterCallback<MouseUpEvent>((evt) => AddFrameEvent());
        removeFrameEventButton.RegisterCallback<MouseUpEvent>((evt) => RemoveFrameEvent());

        animationdataField.RegisterValueChangedCallback(OnAnimationDataChanged);
        modelField.RegisterValueChangedCallback(OnModelChanged);
        animationdataField.visible = false;
        buttonContainer.visible = false;
        frameInspector.visible = false;

        var updateButton = new Button();
        updateButton.text = "Update";
        updateButton.clicked += () =>
        {
            PaintCollisionData();
            PlayAnimationAtFrame();
        };
        root.Add(updateButton);
    }

    void PaintCollisionData()
    {
        if (model == null && currentAnimationData == null)
            return;
        if (animationWindow != null)
        {
            animationWindow.previewing = false;
            animationWindow.Repaint();
        }

        bool open = currentFrameEvent.Type() == AnimationData.EventType.Open;

        for (int i = 0; i < currentFrameEvent.Hitboxes().Count + 1; i++)
        {
            if (i == 1 || !open)
            {
                foreach (Hitbox hitbox in model.GetComponent<CharacterModel>().GetHitboxes())
                {
                    hitbox.SetState(ColliderState.Closed);
                }
            }
            if (i == 0)
                continue;
            if (open)
            {
                model.GetComponent<CharacterModel>().GetHitboxes()[i - 1].SetState(ColliderState.Open);
                model.GetComponent<CharacterModel>().GetHitboxes()[i - 1].SetSize(currentFrameEvent.Hitboxes()[i - 1].Size());
                model.GetComponent<CharacterModel>().GetHitboxes()[i - 1].SetPosition(currentFrameEvent.Hitboxes()[i - 1].Position());
            }
        }

        for (int i = 0; i < currentFrameEvent.Hurtboxes().Count + 1; i++)
        {
            if (i == 0)
                continue;
            if (i == 1)
            {
                foreach (Hurtbox hurtbox in model.GetComponent<CharacterModel>().GetHurtboxes())
                {
                    hurtbox.SetState(ColliderState.Closed);
                }
            }
            model.GetComponent<CharacterModel>().GetHurtboxes()[i - 1].SetState(ColliderState.Open);
            model.GetComponent<CharacterModel>().GetHurtboxes()[i - 1].SetSize(currentFrameEvent.Hurtboxes()[i - 1].Size());
            model.GetComponent<CharacterModel>().GetHurtboxes()[i - 1].SetPosition(currentFrameEvent.Hurtboxes()[i - 1].Position());
        }
    }

    void OnAnimationDataChanged(ChangeEvent<Object> evt)
    {

        if (evt.newValue == null)
        {
            buttonContainer.visible = false;
            frameInspector.visible = false;
            Debug.Log("value is null");
            return;
        }
        currentAnimationData = evt.newValue as AnimationData;
        PopulateList();
        buttonContainer.visible = true;
        frameInspector.visible = true;

    }

    void OnModelChanged(ChangeEvent<Object> evt)
    {
        if (evt.newValue == null)
        {
            Debug.Log("value is null");
            animationdataField.visible = false;
            return;
        }
        if (evt.newValue.GetComponent<CharacterModel>() == null)
        {
            Debug.LogError("Prefab is missing CharacterModel");
            modelField.SetValueWithoutNotify(null);
            return;
        }
        string prefabPath = AssetDatabase.GetAssetPath((GameObject)evt.newValue);
        PrefabStage prefabStage = PrefabStageUtility.OpenPrefab(prefabPath);
        model = prefabStage.prefabContentsRoot;
        animationdataField.visible = true;


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
            selectedFrameIndex = frameEventList.selectedIndex;
            currentFrameEvent = currentAnimationData.FrameEvents()[selectedFrameIndex];
            UpdateFrameInspector();
            PaintCollisionData();
            PlayAnimationAtFrame();
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
        currentAnimationData.FrameEvents().RemoveAt(selectedFrameIndex);
        frameEventList.Rebuild();
    }

    void UpdateFrameInspector()
    {
        SerializedObject serializedObject = new SerializedObject(currentAnimationData);

        SerializedProperty listProperty = serializedObject.FindProperty("frameEvents");
        SerializedProperty selectedFrame = listProperty.GetArrayElementAtIndex(selectedFrameIndex) as SerializedProperty;
        propertyField.BindProperty(selectedFrame);
    }
    private void PlayAnimationAtFrame()
    {
        AnimationClip targetClip = currentAnimationData.AnimationClip();
        if (targetClip == null) return;
        GetAnimationWindow();
        if (animationWindow != null)
        {
            animationWindow.animationClip = targetClip;
            animationWindow.playing = true;
            animationWindow.previewing = true;
            animationWindow.playing = false;
            animationWindow.time = (currentFrameEvent.Frame / 60f);
            animationWindow.Repaint();
        }
    }

    private void GetAnimationWindow()
    {
        if (animationWindow == null) TryGetAnimationWindow();
    }

    private bool TryGetAnimationWindow()
    {
        animationWindow = EditorWindow.GetWindow<AnimationWindow>();
        return animationWindow != null;
    }

}

