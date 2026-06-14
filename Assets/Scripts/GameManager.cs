using SkillIssue.CharacterSpace;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI frameDisplay;
    [SerializeField]
    TextMeshProUGUI frameScriptDisplay;
    [SerializeField]
    GeneralCombatValues generalCombatValues;
    public bool IsTrainingModeOn { get; private set; }
    [SerializeField]
    bool toggleTraining = false;
    public Player CornerPlayer { get; private set; }
    [SerializeField]
    Player[] players;
    [SerializeField]
    BattleUI battleUI;
    bool isGamePaused = false;
    [SerializeField]
    float gameSpeed = 1.0f;
    public int RecordingFrame { get; private set; }
    public bool IsRecording { get; private set; }

    public int frame = 0;
    public bool countframes = false;
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 1f / 60f;
    }
    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i <= players.Length -1 ; i++)
        {
            Managers.Instance.InputManager.SetupController(i);
            Managers.Instance.InputManager.SwitchToMap("Controls");
        }
        IsTrainingModeOn = toggleTraining;
        Time.timeScale = gameSpeed;

    }
    private void FixedUpdate()
    {
        float fps = 1f / Time.unscaledDeltaTime;
        frameDisplay.text = "FPS: " + Mathf.RoundToInt(fps);
        if (IsRecording)
        {
            RecordingFrame++;
            Debug.Log("Recording");
        }
        if (countframes)
        {
            frame++;
            frameScriptDisplay.text = "Frame: " + frame;
        }
    }

    public Player[] GetPlayers()
    {
        return players;
    }

    public GeneralCombatValues GetCombatValues()
    {
        return generalCombatValues;
    }

    public void ToggleRecording()
    {
        IsRecording = !IsRecording;
        RecordingFrame = 0;
    }

    public void SetCornerChar(Player character)
    {
        CornerPlayer = character;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PauseGame()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;
        foreach (Player player in players) 
        {
            player.GetCharacterAnimation().SetPlayspeed(Time.timeScale);
        }
        if (battleUI != null)
            battleUI.ShowPauseUI(isGamePaused);
    }

    public void ResetPosition()
    {
        // Don't reload screen
        battleUI.ResetAll();
    }

    public void EnableTrainingMode()
    {
        IsTrainingModeOn = !IsTrainingModeOn;
        //character2.inputHandler.ResetAI();
    }

    //Maybe Event(?)
    public void UpdateHealth(int playerId, float value)
    {
        if (battleUI == null || IsTrainingModeOn)
            return;
        battleUI.UpdateHealth(playerId, value);
    }

    //Maybe Event(?)
    public void UpdateComboCounter(int playerId)
    {
        if (battleUI == null)
            return;
        battleUI.UpdateComboCounter(playerId);
    }

    public void SetBattleUI(BattleUI uI)
    {
        battleUI = uI;
        battleUI.Initialize();
        battleUI.FadeIn();
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
