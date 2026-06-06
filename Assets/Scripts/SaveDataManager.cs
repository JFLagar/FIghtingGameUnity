using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEditor;

public class SaveDataManager : MonoBehaviour
{
    [System.Serializable]
    public class ActionMap
    {
        public string action;
        public string id;
        public string path;
        public string interactions;
        public string processors;
    }

    public static SaveDataManager Instance;
    private string saveDataPath;
    public UserData ActiveSaveData { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    private void Start()
    {
        saveDataPath = Application.persistentDataPath + "/data.json";
    }

    public void SaveData(UserData m_data = null)
    {
        if (m_data == null)
        {
            m_data = ActiveSaveData;
        }
        string json = JsonUtility.ToJson(m_data);
        File.WriteAllText(saveDataPath, json);
        json = File.ReadAllText(saveDataPath);
        ActiveSaveData = JsonUtility.FromJson<UserData>(json);
    }

    public bool CheckData()
    {
        saveDataPath = Application.persistentDataPath + "/data.json";

        if (File.Exists(saveDataPath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SaveInputData(int controllerID)
    {
        PlayerController[] playerControllers = InputManager.Instance.GetPlayerControllers();
        for (int i = 0; i <= playerControllers.Length - 1; i++)
        {
            string json = playerControllers[i].GetPlayerInput().actions.SaveBindingOverridesAsJson();
            // check if theres a save for the ID
            InputUserData inputUserData = ActiveSaveData.InputUserDatas.FirstOrDefault(c => c.ControllerID == playerControllers[i].Id);
            if (inputUserData != null)
            {
                inputUserData.InputMaps = json;
            }
            else
            {
                inputUserData = new InputUserData(playerControllers[i].Id, json);
                ActiveSaveData.InputUserDatas.Add(inputUserData);
            }
        }
        SaveData(ActiveSaveData);
    }

    public void LoadData()
    {
        if (CheckData())
        {
            string json = File.ReadAllText(saveDataPath);
            ActiveSaveData = JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            CreateNewSave();
            Debug.Log("Creating data");
        }
        LoadInputData();
    }

    public void CreateNewSave()
    {
        GameSettingsData defaultSettings = new GameSettingsData();
        defaultSettings = Resources.Load<GameSettings>("DefaultGameSettings").Data;
        defaultSettings.m_DisplaySettings.Resolutions = GetScreenResolutions();
        defaultSettings.m_DisplaySettings.ResolutionId = defaultSettings.m_DisplaySettings.Resolutions.Length - 1;
        SaveData(new UserData(defaultSettings));
    }

    private Vector2[] GetScreenResolutions()
    {
        List<Vector2> resolutionsList = new List<Vector2>();
        // getting only the 16:9 resolutions
        foreach (var resolution in Screen.resolutions)
        {
            Vector2 resolutionVector = new Vector2(resolution.width, resolution.height);
            resolutionsList.Add(resolutionVector);
        }

        Vector2[] resolutions = resolutionsList.ToArray();
        return resolutions;
    }

    public void LoadInputData()
    {
        PlayerController[] playerControllers = InputManager.Instance.GetPlayerControllers();
        for (int i = 0; i <= playerControllers.Length - 1; i++)
        {
            // check if theres a save for the ID
            InputUserData inputUserData = ActiveSaveData.InputUserDatas.FirstOrDefault(c => c.ControllerID == playerControllers[i].Id);
            if (inputUserData != null)
            {
                playerControllers[i].GetPlayerInput().actions.LoadBindingOverridesFromJson(inputUserData.InputMaps);
                InputAction action = new InputAction();
            }
        }
    }
}

[System.Serializable]
public class UserData
{
    public List<InputUserData> InputUserDatas = new List<InputUserData>();
    
    public GameSettingsData GameSettings;
    public UserData(GameSettingsData gameSettings)
    {
        GameSettings = gameSettings;
    }
}

[System.Serializable]
public class InputUserData
{
    public int ControllerID;
    public string InputMaps;
    public InputUserData(int controllerID, string inputMaps)
    {
        ControllerID = controllerID;
        InputMaps = inputMaps;
    }
}