using NUnit;
using SkillIssue.CharacterSpace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI frameDisplay;
    [SerializeField]
    bool isTraining;
    [SerializeField]
    Character cornerCharacter;
    int p1rounds;
    int p2rounds;
    [SerializeField]
    UIBehaviour uIBehaviour;
    bool isGamePaused = false;
    [SerializeField]
    float gameSpeed = 1.0f;
    [SerializeField]
    float generalForceSpeed = 1.0f;
    public int RecordingFrame {  get; private set; }
    public bool IsRecording {  get; private set; }
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 1f / 60f;
    }
    // Start is called before the first frame update
    private void Start()
    {
        if (uIBehaviour != null)
        {
            uIBehaviour.Initialize();
            uIBehaviour.FadeIn();
        }

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
    }

    public void ToggleRecording()
    {
        IsRecording = !IsRecording;
        RecordingFrame = 0;
    }

    public void SetCornerChar(Character character)
    {
        cornerCharacter = character;
    }

    public Character GetCornerChar()
    {
        return cornerCharacter;
    }

    public float GetForceSpeed()
    {
        return generalForceSpeed;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PauseGame()
    {
        if (uIBehaviour == null)
            return;
        isGamePaused = !isGamePaused;
        if (uIBehaviour != null)
            uIBehaviour.ShowPauseUI(isGamePaused);
    }

    public void ResetPosition()
    {
        // Don't reload screen
        uIBehaviour.ResetAll();
    }

    public void EnableTrainingMode()
    {
        isTraining = !isTraining;
        //character2.inputHandler.ResetAI();
    }

    //Maybe Event(?)
    public void UpdateHealth(int playerId, float value)
    {
        if (uIBehaviour == null || IsTraining())
            return;
        uIBehaviour.UpdateHealth(playerId, value);
    }

    //Maybe Event(?)
    public void UpdateComboCounter(int playerId)
    {
        if (uIBehaviour == null)
            return;
        uIBehaviour.UpdateComboCounter(playerId);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public bool IsTraining()
    {
        return isTraining;
    }

}
