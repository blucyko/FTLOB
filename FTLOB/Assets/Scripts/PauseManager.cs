using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputActionReference togglePauseAction;

    // [SerializeField] private string currentScene;
    [SerializeField] private string MenuScene = "TitleScreen";

    private bool pauseActive;

    // private void OnEnable()
    // {
    //     if (togglePauseAction != null)
    //     {
    //         togglePauseAction.action.performed += TogglePause;
    //         togglePauseAction.action.Enable();
    //     }
    // }

    // private void OnDisable()
    // {
    //     if (togglePauseAction != null)
    //     {
    //         togglePauseAction.action.performed -= TogglePause;
    //         togglePauseAction.action.Disable();
    //     }
    // }

    void Start()
    {
        if (togglePauseAction != null)
        {
            // links the TogglePause function to the togglePauseAction InputActionReference, 
            // so that when the button for it is pressed it calls togglePauseAction
            togglePauseAction.action.performed += TogglePause; 
            togglePauseAction.action.Enable();
        }
        pausePanel.SetActive(false);
        pauseActive = pausePanel.activeSelf;
    }




    public void TogglePause(InputAction.CallbackContext context)
    {
        if (pausePanel == null)
        {
            return;
        }

        ResumeButton();
    }

    public void ResumeButton()
    {
        // Don't toggle pause menu if options panel is open
        if (optionsPanel != null && optionsPanel.activeSelf)
        {
            return;
        }

        pauseActive = !pauseActive;
        Debug.Log("Is pause panel active? " + pauseActive);
        pausePanel.SetActive(pauseActive);
        Time.timeScale = pauseActive ? 0f : 1f; // Pause or resume the game based on pause panel state
        Debug.Log(pauseActive ? "Game Paused" : "Game Resumed");
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
        return;
    }

    public void OptionsButton()
    {
        return;
    }

    public void ControlsButton()
    {
        return;
    }

    public void ExitButton()
    {
        return;
    }

}
