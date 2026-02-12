using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    [Header("Audio & Visual")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
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

    [Header("SFX Library")]
    public AudioClip jumpSound;
    public AudioClip collectSound;
    public AudioClip deathSound;
    public AudioClip winSound;
    public AudioClip clickSound;

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
        AssignButtonSounds();
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
        // Поиск музыки и SFX на новой сцене
        if (musicSource == null)
        {
            GameObject musicObj = GameObject.Find("music");
            if (musicObj != null) musicSource = musicObj.GetComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = GameObject.Find("sfx");
            if (sfxObj != null) sfxSource = sfxObj.GetComponent<AudioSource>();
        }
        
        SetupSliders();
        ApplyLoadedSettings(); 
        AssignButtonSounds();
    }

    // --- ОСНОВНЫЕ МЕТОДЫ ---

    private void AssignButtonSounds()
    {
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveListener(PlayClickSound); 
            btn.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound() 
    {
        PlaySFX(clickSound);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    private void ApplyLoadedSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        stabilizationValue = PlayerPrefs.GetFloat("Stabilization", 5f);

        if (musicSource != null)
        {
            musicSource.volume = savedVolume;
            if (!musicSource.isPlaying) musicSource.Play();
        }
    }

    private void SetupSliders()
    {
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

    // --- ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ КНОПОК (НЕ МЕНЯТЬ НАЗВАНИЯ) ---

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
        if (musicSource != null) PlayerPrefs.SetFloat("MusicVolume", musicSource.volume);
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

    public void ToggleSettings(bool isOpen)
    {
        if (settingsPanel == null)
            settingsPanel = GameObject.Find("Canvas")?.transform.Find("SettingsPanel")?.gameObject;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(isOpen);
            Time.timeScale = isOpen ? 0f : 1f;
            if (isOpen) SetupSliders();
        }
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

    public void ToggleSound()
    {
        if (musicSource == null) return;
        float newVol = musicSource.volume > 0 ? 0 : 0.5f;
        ApplyVolume(newVol);
        if (volumeSlider != null) volumeSlider.value = newVol;
        if (soundIconImage != null) soundIconImage.sprite = newVol > 0 ? soundOnSprite : soundOffSprite;
    }

    public void ContinueScene() => TogglePause(false);
    public void LoadSpecificScene(string sceneName) => SceneManager.LoadScene(sceneName);

    // Вызывай этот метод, когда игрок заходит на уровень (в Start уровня)
public void SaveCurrentLevelName()
{
    string name = SceneManager.GetActiveScene().name;
    if (name != "GameOver" && name != "MainMenu")
    {
        PlayerPrefs.SetString("LastLevel", name);
        PlayerPrefs.Save();
    }
}

public void RestartLevel()
{
    Time.timeScale = 1f;
    string currentScene = SceneManager.GetActiveScene().name;

    // Если мы в меню GameOver, нужно понять, какой уровень перезагрузить.
    // Если же мы на самом уровне (через меню паузы), просто перезагружаем текущую сцену.
    if (currentScene == "GameOver")
    {
        // Загружаем сохраненное имя последнего уровня (или HUD по умолчанию)
        string lastLevel = PlayerPrefs.GetString("LastLevel", "HUD"); 
        SceneManager.LoadScene(lastLevel);
    }
    else
    {
        // Если нажали "Restart" в меню паузы прямо на уровне
        SceneManager.LoadScene(currentScene);
    }
}

    public void GoToMainMenu() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}