using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    AudioSource _audio;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            _audio.Stop();
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (StaticData.currentState == GameState.Game)
        {
            Instance = null;
            Destroy(gameObject);
            _audio.Stop();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += DestroyIfGame;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= DestroyIfGame;
    }

    void DestroyIfGame(Scene scene, LoadSceneMode mode)
    {
        if (StaticData.currentState == GameState.Game)
        {
            Instance = null;
            _audio.Stop();
            Destroy(gameObject);
        }
    }
}
