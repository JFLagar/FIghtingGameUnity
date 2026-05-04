using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager instance;
    private string saveDataPath;
    public UserData ActiveSaveData { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        saveDataPath = Application.persistentDataPath + "/data.json";

        if (CheckData())
        {
            Debug.Log("Exists");
            string json = File.ReadAllText(saveDataPath);
            ActiveSaveData = JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            Debug.Log("Doesn't Exist");
            SaveData(new UserData());
        }
    }
    public void SaveData(UserData m_data)
    {
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
}