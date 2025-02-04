using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance;
    public GameManager GameManager { get; private set; }
    public CameraManager CameraManager { get; private set; }
    public AudioManager AudioManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(this);
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        GameManager = FindFirstObjectByType<GameManager>();
        CameraManager = FindFirstObjectByType<CameraManager>();
        AudioManager = FindFirstObjectByType<AudioManager>();
    }
}
