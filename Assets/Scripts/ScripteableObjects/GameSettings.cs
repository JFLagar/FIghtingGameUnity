using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    public GameSettingsData Data;
}

[System.Serializable]
public struct GameSettingsData
{
    public AudioSettings m_AudioSettings;
    public SystemSettings m_SystemSettings;
    public DisplaySettings m_DisplaySettings;
    public BattleSettings m_BattleSettings;
}

[Serializable]
public struct AudioSettings
{
    public float MusicVolume;
    public float SFXVolume;
}

[Serializable]
public struct SystemSettings
{

}

[Serializable]
public struct DisplaySettings
{
    public FullScreenMode WindowMode;
    public Vector2 Resolution;
}

[Serializable]
public struct BattleSettings
{
    public int RoundsId;
    public int[] Rounds;
    public int TimerId;
    public int[] Timers;
}
