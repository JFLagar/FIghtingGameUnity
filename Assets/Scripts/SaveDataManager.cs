using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;
using System.Linq;

public class SaveDataManager : MonoBehaviour
{
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

    public void SaveInputData()
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
                inputUserData = new InputUserData(playerControllers[i].Id,0,json);
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
            SaveData(new UserData());
        }
        LoadInputData();
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
            }
        }
    }
}

[System.Serializable]
public class UserData
{
    public List<InputUserData> InputUserDatas = new List<InputUserData>();
}

[System.Serializable]
public class InputUserData
{
    public int ControllerID;
    public int ProfileID;
    public string InputMaps;
    public InputUserData(int controllerID, int profileID, string inputMaps)
    {
        ControllerID = controllerID;
        ProfileID = profileID;
        InputMaps = inputMaps;
    }
}