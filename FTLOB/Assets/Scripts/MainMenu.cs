using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputActionReference togglePauseAction;

    [SerializeField] private string startScene = "Ship";
    [SerializeField] private string titleScene = "TitleScreen";
    [SerializeField] private string tutorialScene = "Tutorial";
    [SerializeField] private string creditsScene = "Credits";

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

    public void StartGame()
    {
        // Current scene loading for continue and new game button, change later to either continue from where the player left off or start a new game
        Time.timeScale = 1f; // Ensure the game is running when starting
        SceneManager.LoadScene(startScene);
    }

    public void Titlescreen()
    {
        Time.timeScale = 1f; // Ensure the game is running when returning to title screen
        SceneManager.LoadScene(titleScene);
    }

    public void Tutorial()
    {
        Time.timeScale = 1f; // Ensure the game is running when starting tutorial
        SceneManager.LoadScene(tutorialScene);
    }

    public void ToggleOptions()
    {
        if (optionsPanel == null)
        {
            return;
        }
        bool open = !optionsPanel.activeSelf;
        optionsPanel.SetActive(open);

        if (open && pausePanel != null)
        {
            pausePanel.SetActive(false); // Close pause menu when opening options
        }
        Time.timeScale = open ? 0f : 1f; // Pause or resume the game based on options panel state
    }

    public void TogglePause()
    {
        if (pausePanel == null)
        {
            return;
        }

        // Don't toggle pause menu if options panel is open
        if (optionsPanel != null && optionsPanel.activeSelf)
        {
            return;
        }

        bool open = !pausePanel.activeSelf;
        pausePanel.SetActive(open);
        Time.timeScale = open ? 0f : 1f; // Pause or resume the game based on pause panel state
        Debug.Log(open ? "Game Paused" : "Game Resumed");
    }

    public void Credits()
    {
        Time.timeScale = 1f; // Ensure the game is running when showing credits
        SceneManager.LoadScene(creditsScene);
    }

    public void Quit()
    {
        Debug.Log("Quit Game");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}