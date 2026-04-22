using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance;
    public GameManager GameManager { get; private set; }
    public CameraManager CameraManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public InputManager InputManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Add not destroy on load when starting the game in the splash screen scene
        }
        else
            DestroyImmediate(this);
        Initialize();
    }

    public void Initialize()
    {
        GameManager = FindFirstObjectByType<GameManager>();
        CameraManager = FindFirstObjectByType<CameraManager>();
        AudioManager = FindFirstObjectByType<AudioManager>();
        InputManager = FindFirstObjectByType<InputManager>();
    }
}
