using UnityEngine;

public abstract class Anomaly : MonoBehaviour
{
    [Header("Anomaly Settings")]
    [SerializeField] protected string anomalyName;

    [Header("Anomaly References")]
    [SerializeField] protected GameObject anomalyObject;
    [SerializeField] protected GameObject stressMeter;

    protected AnomalyStress stressSystem;
    protected ShipRooms shipRooms;

    public AnomalyRoom AssignedRoom { get; private set; } = AnomalyRoom.None;

    public bool IsStressed { get; protected set; }
    public bool IsManagementActive { get; protected set; }

    protected virtual void Awake()
    {
        stressSystem = GetComponent<AnomalyStress>();
        shipRooms = FindFirstObjectByType<ShipRooms>();

        if (stressSystem != null)
            stressSystem.MaxStressReached += MaxStress;

        if (anomalyObject != null)
            anomalyObject.SetActive(true);

        if (stressMeter != null)
            stressMeter.SetActive(true);
    }

    protected virtual void OnDestroy()
    {
        if (stressSystem != null)
            stressSystem.MaxStressReached -= MaxStress;
    }

    // Assign anomaly to a room
    public bool AssignRoom(AnomalyRoom newRoom)
    {
        if (shipRooms == null)
            return false;

        return shipRooms.AssignAnomaly(this, newRoom);
    }

    // Called by ShipRooms when room assignment changes
    public void SetAssignedRoom(AnomalyRoom newRoom)
    {
        // Remove the previous room's buff
        if (!IsStressed)
            DisableBuffs();

        AssignedRoom = newRoom;

        // Apply the new room's buff
        if (!IsStressed)
            EnableBuffs();
    }

    // Called when the anomaly reaches its maximum stress level
    protected virtual void MaxStress()
    {
        IsStressed = true;
        IsManagementActive = true;
        // Disables Anomaly buffs while it is stressed
        DisableBuffs();
        ManagementStart();
    }

    // Each anomaly implements its own management task associated with comforting it
    protected abstract void ManagementStart();

    // Each anomaly implements its own buffs
    protected abstract void EnableBuffs();
    protected abstract void DisableBuffs();

    // Called when the management task is completed
    public virtual void ManagementCompleted()
    {
        if (!IsManagementActive)
            return;

        IsStressed = false;
        IsManagementActive = false;
        // Reset stress system once anomaly is comforted
        if (stressSystem != null)
            stressSystem.ResetStress();

        EnableBuffs();
    }

    // Called when battle begins
    public virtual void OnCombatStarted()
    {
        // Override in anomalies with active abilities
    }
}
