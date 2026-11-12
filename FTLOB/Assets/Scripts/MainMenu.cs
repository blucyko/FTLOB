using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string startScene = "Ship";
    [SerializeField] private string titleScene = "TitleScreen";
    [SerializeField] private string tutorialScene = "Tutorial";
    [SerializeField] private string creditsScene = "Credits";

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

    //public void ToggleOptions()
    //{
    //    if (optionsPanel == null)
    //    {
    //        return;
    //    }
    //    bool open = !optionsPanel.activeSelf;
    //    optionsPanel.SetActive(open);

    //    if (open && pausePanel != null)
    //    {
    //        pausePanel.SetActive(false); // Close pause menu when opening options
    //    }
    //    Time.timeScale = open ? 0f : 1f; // Pause or resume the game based on options panel state
    //}

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