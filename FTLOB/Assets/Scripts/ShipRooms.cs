using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class AnomalyAssignment
{
    public AnomalyRoom room;
    public Anomaly anomaly;
}

public class ShipRooms : MonoBehaviour
{
    [Header("Room Tilemaps")]
    [SerializeField] private Tilemap shieldRoom;
    [SerializeField] private Tilemap engineRoom;
    [SerializeField] private Tilemap weaponsRoom;
    [SerializeField] private Tilemap lensRoom;

    [SerializeField] private AnomalyAssignment[] startingAssignments;

    private Dictionary<AnomalyRoom, Anomaly> assignedAnomalies = new Dictionary<AnomalyRoom, Anomaly>();
    private bool isCombatActive;
    public bool IsCombatActive => isCombatActive;

    private void Awake()
    {
        // Initialize available rooms.
        assignedAnomalies.Add(AnomalyRoom.Shield, null);
        assignedAnomalies.Add(AnomalyRoom.Engine, null);
        assignedAnomalies.Add(AnomalyRoom.Weapons, null);
        assignedAnomalies.Add(AnomalyRoom.Lens, null);

        // Load starting assignments.
        if (startingAssignments == null)
            return;

        foreach (AnomalyAssignment assignment in startingAssignments)
        {
            if (assignment.anomaly == null || assignment.room == AnomalyRoom.None)
                continue;

            // Check whether the room exists.
            if (!assignedAnomalies.ContainsKey(assignment.room))
            {
                Debug.LogWarning("Invalid anomaly room: " + assignment.room);
                continue;
            }

            // Check whether the room is occupied.
            if (assignedAnomalies[assignment.room] != null)
            {
                Debug.LogWarning("Room already has an assigned anomaly: " + assignment.room);
                continue;
            }

            // Check whether the anomaly is already assigned.
            if (IsAnomalyAssigned(assignment.anomaly))
            {
                Debug.LogWarning("Anomaly already assigned: " + assignment.anomaly.name);
                continue;
            }

            assignedAnomalies[assignment.room] = assignment.anomaly;
        }
    }

    private void Start()
    {
        // Apply starting assignments after all Awake methods finish.
        foreach (var assignment in assignedAnomalies)
        {
            if (assignment.Value != null)
            {
                assignment.Value.SetAssignedRoom(assignment.Key);

                Debug.Log(assignment.Value.name + " assigned to " + assignment.Key
                );
            }
        }
    }

    // Assign an anomaly to an empty room.
    public bool AssignAnomaly(Anomaly anomaly, AnomalyRoom room)
    {
        if (isCombatActive)
        {
            Debug.Log("Cannot move anomalies during combat.");
            return false;
        }

        if (anomaly == null || room == AnomalyRoom.None)
            return false;

        if (!assignedAnomalies.ContainsKey(room))
            return false;

        // Don't reassign to the same room.
        if (anomaly.AssignedRoom == room)
            return true;

        // Only allow assignment to an empty room.
        if (assignedAnomalies[room] != null)
            return false;

        // Find the anomaly's previous room.
        AnomalyRoom previousRoom = anomaly.AssignedRoom;

        // Remove it from its previous room.
        if (previousRoom != AnomalyRoom.None)
        {
            if (!assignedAnomalies.ContainsKey(previousRoom) || assignedAnomalies[previousRoom] != anomaly)
            {
                Debug.LogWarning("Anomaly assignment is inconsistent.");
                return false;
            }

            assignedAnomalies[previousRoom] = null;
        }

        // Assign it to the new room.
        assignedAnomalies[room] = anomaly;

        // Update the anomaly's room and buffs.
        anomaly.SetAssignedRoom(room);

        Debug.Log(anomaly.name + " moved to " + room);

        return true;
    }

    // Swap two anomalies between their assigned rooms.
    public bool SwapAnomalies(Anomaly first, Anomaly second)
    {
        if (isCombatActive)
        {
            Debug.Log("Cannot swap anomalies during combat.");
            return false;
        }

        if (first == null || second == null || first == second)
            return false;

        AnomalyRoom firstRoom = first.AssignedRoom;
        AnomalyRoom secondRoom = second.AssignedRoom;

        if (firstRoom == AnomalyRoom.None || secondRoom == AnomalyRoom.None)
            return false;

        // Verify both assignments.
        if (!assignedAnomalies.ContainsKey(firstRoom) || !assignedAnomalies.ContainsKey(secondRoom))
            return false;

        if (assignedAnomalies[firstRoom] != first || assignedAnomalies[secondRoom] != second)
            return false;

        // Swap dictionary assignments.
        assignedAnomalies[firstRoom] = second;
        assignedAnomalies[secondRoom] = first;

        // Update both anomalies.
        first.SetAssignedRoom(secondRoom);
        second.SetAssignedRoom(firstRoom);

        Debug.Log("Swapped " + first.name + " and " + second.name);
        return true;
    }

    // Retrieve the anomaly installed in a particular room.
    public Anomaly GetAnomaly(AnomalyRoom room)
    {
        if (assignedAnomalies.TryGetValue(room, out Anomaly anomaly))
        {
            return anomaly;
        }

        return null;
    }

    // Check whether an anomaly is already assigned.
    private bool IsAnomalyAssigned(Anomaly anomaly)
    {
        return assignedAnomalies.ContainsValue(anomaly);
    }

    // Called when combat begins or ends.
    public void SetCombatActive(bool active)
    {
        // Prevent repeated initialization.
        if (isCombatActive == active)
            return;

        isCombatActive = active;

        if (!active)
        {
            Debug.Log("Combat ended. Anomaly assignments unlocked.");
            return;
        }

        Debug.Log("Combat started. Anomaly assignments locked.");

        // Initialize cooldowns for installed anomalies.
        foreach (Anomaly anomaly in assignedAnomalies.Values)
        {
            if (anomaly != null)
                anomaly.OnCombatStarted();
        }
    }

    // Crew Member Room Detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckRoom(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckRoom(collision, false);
    }

    private void CheckRoom(Collider2D collision, bool entering)
    {
        int layer = collision.gameObject.layer;

        if (layer == LayerMask.NameToLayer("Shield Room"))
        {
            Debug.Log(entering ? "Player is in the Shield Room" : "Player has left the Shield Room");
        }
        else if (layer == LayerMask.NameToLayer("Engine Room"))
        {
            Debug.Log(entering ? "Player is in the Engine Room" : "Player has left the Engine Room");
        }
        else if (layer == LayerMask.NameToLayer("Weapons Room"))
        {
            Debug.Log(entering ? "Player is in the Weapons Room" : "Player has left the Weapons Room");
        }
        else if (layer == LayerMask.NameToLayer("Lens Room"))
        {
            Debug.Log(entering ? "Player is in the Lens Room" : "Player has left the Lens Room");
        }
    }
}