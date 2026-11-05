using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private InputActionReference togglePauseAction;

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
    }

    // private void OnTogglePause(InputAction.CallbackContext context)
    // {
    //     TogglePause();
    // }


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

        bool open = !pausePanel.activeSelf;
        pausePanel.SetActive(open);
        Time.timeScale = open ? 0f : 1f; // Pause or resume the game based on pause panel state
        Debug.Log(open ? "Game Paused" : "Game Resumed");
    }

    public void MainMenuButton()
    {
        return;
    }

    public void RestartButton()
    {
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
