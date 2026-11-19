using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _battleMapPanel;

    private void Awake()
    {
        if (_battleMapPanel)
        {
            _battleMapPanel.SetActive(false);
        }
    }
    public void BeaconMap()
    {
        // Load the Beacon Map scene
        // !!(later add condition to check if ship has enough fuel to jump)
        SceneManager.LoadScene("NodeNav");
    }

    // Test button for opening battle map panel
    public void BattleMap()
    {
        // Load the Battle Map scene
        if (_battleMapPanel != null)
        {
            Debug.Log("Opening Battle Map Panel");
            // Toggle the active state of the battle map panel
            _battleMapPanel.SetActive(!_battleMapPanel.activeSelf);
        }
    }
}
