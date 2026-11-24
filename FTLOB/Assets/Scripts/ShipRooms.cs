using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipRooms : MonoBehaviour
{
    [Header("Room Tilemaps")]
    [SerializeField] private Tilemap shieldRoom;
    [SerializeField] private Tilemap engineRoom;
    [SerializeField] private Tilemap weaponsRoom;
    [SerializeField] private Tilemap lensRoom;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check the layer of the colliding tilemap to determine what room of the ship the player is in
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shield Room"))
        {
            Debug.Log("Player is in the Shield Room");
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Engine Room"))
        {
            Debug.Log("Player is in the Engine Room");
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Weapons Room"))
        {
            Debug.Log("Player is in the Weapons Room");
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Lens Room"))
        {
            Debug.Log("Player is in the Lens Room");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check the layer of the colliding tilemap to determine what room of the ship the player is leaving
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shield Room"))
        {
            Debug.Log("Player has left the Shield Room");
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Engine Room"))
        {
            Debug.Log("Player has left the Engine Room");
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Weapons Room"))
        {
            Debug.Log("Player has left the Weapons Room");
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Lens Room"))
        {
            Debug.Log("Player has left the Lens Room");
        }

    }
}
