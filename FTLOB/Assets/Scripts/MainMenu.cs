using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Assemblies;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputActionReference togglePauseAction;

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
        SceneManager.LoadScene("Ship");
    }

    public void Titlescreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void ToggleOptions()
    {
        if (optionsPanel != null)
        {
            bool isActive = optionsPanel.activeSelf;
            optionsPanel.SetActive(!isActive);
        }

        if (pausePanel != null && optionsPanel.activeSelf)
        {
            pausePanel.SetActive(false); // Close pause menu when opening options
        }

        // Resume the game if options are closed
        if (!optionsPanel.activeSelf)
        {
            Time.timeScale = 1f; // Resume the game
        }
        else
        {
            Time.timeScale = 0f; // Pause the game when options are open
        }
    }

    public void TogglePause()
    {
        if (pausePanel != null)
        {
            Debug.Log("Toggling pause menu");
            bool isActive = pausePanel.activeSelf;
            Time.timeScale = isActive ? 1f : 0f; // Resume or pause the game
            if (optionsPanel == isActive)
            {
                pausePanel.SetActive(!isActive); // Close pause menu when opening options
            }
            else
            {
                pausePanel.SetActive(!isActive); // Toggle pause menu
            }
        }
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
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