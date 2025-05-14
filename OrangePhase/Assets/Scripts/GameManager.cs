using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }
    public PlayerMovement Player { get; set; }
    public SaveSystem PlayerSaveData { get; set; }
    public SavePointController SavePointController { get; set; }

    [Header("UI Menus")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;


    [Header("Options Menu Elements")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] availableResolutions;
    private bool isGamePaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetupResolutionOptions();
        SetupQualityOptions();
        SetupVolumeSlider();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isGamePaused) return;

        Time.timeScale = 0f;
        isGamePaused = true;

        pauseButton.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!isGamePaused) return;

        Time.timeScale = 1f;
        isGamePaused = false;

        pauseMenu.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Closing Game");
        Application.Quit();
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public void ChangeScene(string sceneName)
    {
        Debug.Log("Scene Changed");
        SceneManager.LoadScene(sceneName);
    }

    public void Save()
    {
        SaveSystem.Save();
    }

    public void Load()
    {
        SaveSystem.Load();
    }

    public void CagesHandler()
    {
        bool mainMenuState;
        mainMenuState = mainMenu.activeInHierarchy;


        if (mainMenuState)
        {
            mainMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(true);
        }
        else
        {
            optionsMenu.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(true);
        }
    }


    // -----------------------
    // Opciones - Configuración
    // -----------------------

    private void SetupResolutionOptions()
    {
        availableResolutions = Screen.resolutions;


        var options = new System.Collections.Generic.List<string>();
        int currentIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            var res = availableResolutions[i];
            string label = res.width + " x " + res.height;
            options.Add(label);

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                currentIndex = i;
        }


    }

    private void SetupQualityOptions()
    {
        qualityDropdown.ClearOptions();
        string[] qualities = QualitySettings.names;
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(qualities));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    private void SetupVolumeSlider()
    {
        float currentVolume;
        if (audioMixer.GetFloat("Volume", out currentVolume))
            volumeSlider.value = currentVolume;
        else
            volumeSlider.value = 0f; // Default
    }

    public void SetVolume()
    {
        audioMixer.SetFloat("Volume", volumeSlider.value);
    }

    public void SetQuality()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

}