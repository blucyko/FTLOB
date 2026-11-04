using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void BeaconMap()
    {
        // Load the Beacon Map scene
        // !!(later add condition to check if ship has enough fuel to jump)
        SceneManager.LoadScene("NodeNav");
    }
}
