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

    public void Ship()
    {
        // Load Ship Scene
        SceneManager.LoadScene("Ship");
    }

    // Test button for opening battle map panel
    public void BattleMap()
    {
        // Load the Battle Map scene
        SceneManager.LoadScene("DummyEnemy");
        //if (_battleMapPanel != null)
        //{
        //    Debug.Log("Opening Battle Map Panel");
        //    // Toggle the active state of the battle map panel
        //    _battleMapPanel.SetActive(!_battleMapPanel.activeSelf);
        //}
    }

    public void MovementTest()
    {
        // Load Movement test scene
        SceneManager.LoadScene("Ship Movement");
    }
}
