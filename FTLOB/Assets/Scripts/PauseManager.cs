using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputActionReference togglePauseAction;
    [SerializeField] private string MenuScene = "TitleScreen";

    private bool isPaused = false;

    private void Awake()
    {
        if (pausePanel)
        {
            pausePanel.SetActive(false);
        }

        isPaused = false;
        Time.timeScale = 1f; // Ensure the game is running when starting
    }

    private void OnEnable()
    {
        if (togglePauseAction != null)
        {
            togglePauseAction.action.performed += OnTogglePause;
            togglePauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (togglePauseAction != null)
        {
            togglePauseAction.action.performed -= OnTogglePause;
            togglePauseAction.action.Disable();
        }
    }

    private void OnTogglePause(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        Debug.Log("Pause button pressed");
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void MainMenuButton()
    {   
        Time.timeScale = 1f; // Ensure the game is running when returning to title screen
        SceneManager.LoadScene(MenuScene);
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OptionsButton()
    {
        
    }

    public void ControlsButton()
    {
        
    }

    public void ExitButton()
    {
        Debug.Log("Quit Game");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
