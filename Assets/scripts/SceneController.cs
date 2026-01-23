using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [Header("Audio & Visual")]
    public AudioSource musicSource;
    [Range(0.1f, 10f)]
    public float stabilizationValue = 5f;

    [Header("Settings Menu")]
    public GameObject settingsPanel;

    [Header("Pause Menu")]
    public GameObject pausePanel;

    [Header("Icons Mute/Unmute")]
    public Image soundIconImage;    
    public Sprite soundOnSprite;    
    public Sprite soundOffSprite;  

    public Image stabIconImage;    
    public Sprite stabOnSprite;
    public Sprite stabOffSprite;   

    [Header("Sliders")]
    public Slider volumeSlider;
    public Slider stabilizationSlider;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyLoadedSettings();
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
    }

    void Start()
    {
        ApplyLoadedSettings();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Пытаемся найти AudioSource, если он потерялся при переходе
        if (musicSource == null)
        {
            GameObject musicObj = GameObject.Find("music");
            if (musicObj != null) musicSource = musicObj.GetComponent<AudioSource>();
        }
        
        ApplyLoadedSettings(); 
    }

    public void ToggleSettings(bool isOpen)
    {
        if (settingsPanel == null)
        {
            settingsPanel = GameObject.Find("Canvas")?.transform.Find("SettingsPanel")?.gameObject;
            
            if (settingsPanel == null)
            {
                GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (var obj in all)
                {
                    if (obj.name == "SettingsPanel")
                    {
                        settingsPanel = obj;
                        break;
                    }
                }
            }
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isOpen);
            Time.timeScale = isOpen ? 0f : 1f;
            if (isOpen) SetupSliders();
        }
    }

    private void ApplyLoadedSettings()
{
    float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    stabilizationValue = PlayerPrefs.GetFloat("Stabilization", 5f);

    if (musicSource == null)
    {
        GameObject musicObj = GameObject.Find("music");
        if (musicObj != null) musicSource = musicObj.GetComponent<AudioSource>();
    }

    if (musicSource != null)
    {
        musicSource.volume = savedVolume;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
}

    private void SetupSliders()
    {
        // Ищем слайдеры по именам, если ссылки в инспекторе пустые
        if (volumeSlider == null) volumeSlider = GameObject.Find("Slider vol")?.GetComponent<Slider>();
        if (stabilizationSlider == null) stabilizationSlider = GameObject.Find("Slider stab")?.GetComponent<Slider>();

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            volumeSlider.onValueChanged.RemoveAllListeners(); 
            volumeSlider.onValueChanged.AddListener(ApplyVolume);
        }
        
        if (stabilizationSlider != null)
        {
            stabilizationSlider.value = PlayerPrefs.GetFloat("Stabilization", 5f);
            stabilizationSlider.onValueChanged.RemoveAllListeners();
            stabilizationSlider.onValueChanged.AddListener(ApplyStabilization);
        }
    }

    public void ApplyVolume(float val)
    {
        if (musicSource != null) musicSource.volume = val;
    }

    public void ApplyStabilization(float val)
    {
        stabilizationValue = val;
    }

    public void SaveSettings()
    {
        if (musicSource != null)
        {
            PlayerPrefs.SetFloat("MusicVolume", musicSource.volume);
        }
        PlayerPrefs.SetFloat("Stabilization", stabilizationValue);
        PlayerPrefs.Save();
        ToggleSettings(false);
    }

    public void ResetSettings()
    {
        ApplyVolume(0.5f);
        ApplyStabilization(5f);
        PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        PlayerPrefs.SetFloat("Stabilization", 5f);
        PlayerPrefs.Save();
        SetupSliders();
    }

    public void TogglePause(bool isPaused)
    {
        if (pausePanel == null) pausePanel = GameObject.Find("PausePanel");
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused); 
            Time.timeScale = isPaused ? 0f : 1f; 
        }
    }

    public void ContinueScene() => TogglePause(false);

    public void LoadSpecificScene(string sceneName) => SceneManager.LoadScene(sceneName);

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene == "GameOver" ? "HUD" : currentScene);
    }

    public void GoToMainMenu() {
        Time.timeScale = 1f;
        
     SceneManager.LoadScene("MainMenu");
    }

    public void ToggleSound()
    {
        if (musicSource == null) return;
        float newVol = musicSource.volume > 0 ? 0 : 0.5f;
        ApplyVolume(newVol);
        if (volumeSlider != null) volumeSlider.value = newVol;
        if (soundIconImage != null) soundIconImage.sprite = newVol > 0 ? soundOnSprite : soundOffSprite;
    }

    public void ToggleStabilization()
    {
        float newStab = stabilizationValue > 0 ? 0 : 5f;
        ApplyStabilization(newStab);
        if (stabilizationSlider != null) stabilizationSlider.value = newStab;
        if (stabIconImage != null) stabIconImage.sprite = newStab > 0 ? stabOnSprite : stabOffSprite;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}