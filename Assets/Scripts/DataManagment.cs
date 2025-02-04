using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Controllers
{
    keyboard1,
    keyboard2,
    controller1,
    controller2
}
public class DataManagment : MonoBehaviour
{
    public static DataManagment instance;
    public Controllers controllerP1 , controllerP2;
    private string saveData;
    public UserData data;
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
        saveData = Application.persistentDataPath + "/data.json";

        if (File.Exists(saveData))
        {
            Debug.Log("Exists");
            string json = File.ReadAllText(saveData);
            data = JsonUtility.FromJson<UserData>(json);
        }
        else
        {
            Debug.Log("Doesn't Exist");
            ReWriteData(new UserData());
        }
    }
    public void ReWriteData(UserData m_data)
    {
        string json = JsonUtility.ToJson(m_data);
        File.WriteAllText(saveData, json);
        json = File.ReadAllText(saveData);
        data = JsonUtility.FromJson<UserData>(json);


    }
    public bool CheckData()
    {
        saveData = Application.persistentDataPath + "/data.json";

        if (File.Exists(saveData))
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
    public KeyCode[] inputsP1 = new KeyCode[3];
    public KeyCode[] inputsP2 = new KeyCode[3];
    public KeyCode[] controllerP1 = new KeyCode[3];
    public KeyCode[] controllerP2 = new KeyCode[3];
}