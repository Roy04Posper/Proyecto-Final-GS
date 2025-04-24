using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState { playing, pause, win, lose };
    public static GameState gameState;

    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pauseMenu;
    private bool isGamePaused = false;

    private void Start()
    {
        gameState = GameState.playing;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused) { ResumeGame(); }
            else { PauseGame(); }
        }
    }

    public void PauseGame()
    {
        if (isGamePaused) return;
        Time.timeScale = 0f;
        GameManager.gameState = GameState.pause;
        isGamePaused = true;
        Debug.Log("Game paused");

        pauseButton.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        GameManager.gameState = GameState.playing;
        if (!isGamePaused) return;
        Time.timeScale = 1f;
        isGamePaused = false;
        Debug.Log("Resume game");

        pauseMenu.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    public void reStartGame()
    {
        GameManager.gameState = GameState.playing;
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

    public void ChangeScene(string nombre)
    {
        Debug.Log("Scene Changed");
        SceneManager.LoadScene(nombre);
    }
}