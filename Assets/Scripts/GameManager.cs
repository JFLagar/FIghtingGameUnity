using SkillIssue.CharacterSpace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI frameDisplay;
    bool isTraining;
    [SerializeField]
    Character character2;
    int p1rounds;
    int p2rounds;
    [SerializeField]
    UIBehaviour uIBehaviour;
    bool isGamePaused = false;
    private void Awake()
    {
        QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    private void Start()
    {
        if (uIBehaviour != null)
            uIBehaviour.Initialize();
    }

    // Update is called once per frame
    void Update()
    {

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
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
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
