using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class SelectionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Pathfinding pathfinding;

    [Header("Selection")]
    [SerializeField] private string crewTag = "Crew Member";

    private Movement selectedCrew;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current == null || mainCamera == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            HandleRightClick();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float distanceFromCamera = -mainCamera.transform.position.z;

        Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, distanceFromCamera);

        return mainCamera.ScreenToWorldPoint(screenPosition);
    }

    private void HandleLeftClick()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPosition);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(crewTag))
            {
                continue;
            }

            Movement crew = hit.GetComponentInParent<Movement>();

            if (crew != null)
            {
                selectedCrew = crew;
                Debug.Log($"Selected crew member: {crew.gameObject.name}");
                return;
            }
        }

        // Clicking empty space deselects the current crew member.
        selectedCrew = null;
    }

    private void HandleRightClick()
    {
        if (selectedCrew == null)
        {
            Debug.Log("Right-click ignored: no crew member selected.");
            return;
        }

        if (groundTilemap == null || pathfinding == null)
        {
            Debug.LogError("Assign Ground Tilemap and Pathfinding in the Inspector.");
            return;
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        Vector3Int destinationCell = groundTilemap.WorldToCell(mouseWorldPosition);

        // Convert the clicked tile to its world-space center.
        Vector3 destination = groundTilemap.GetCellCenterWorld(destinationCell);

        if (!groundTilemap.HasTile(destinationCell))
        {
            Debug.Log("Destination rejected: no ground tile here.");
            return;
        }

        Vector3Int crewCell = groundTilemap.WorldToCell(selectedCrew.transform.position);
        Vector3 crewCellCenter = groundTilemap.GetCellCenterWorld(crewCell);

        Debug.Log($"Crew world position: {selectedCrew.transform.position} | " + $"Crew cell: {crewCell} | " + $"Cell center: {crewCellCenter}");
        if (!pathfinding.TryFindPath(selectedCrew.transform.position, destination, out var path))
        {
            Debug.Log("No valid path to the selected destination.");
            return;
        }

        selectedCrew.SetPath(path);

        Debug.Log($"Moving {selectedCrew.gameObject.name} to " + $"cell {destinationCell}. Path contains {path.Count} cells.");
    }
}
