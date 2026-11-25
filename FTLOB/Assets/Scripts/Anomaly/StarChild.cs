using UnityEngine;

public class StarChild : Anomaly
{
    // Shield Room Buff: Shield can block an extra shot
    // Engine Room Buff: Speed is Overclocked (2x speed)
    // Weapon buff: Beam Length increased by 1.5x
    // Lens Room Buff(Active Ability): Solar flare. Acts like a very strong ion hit, disabling any system caught in its radius
    // (not your ship) for 4 seconds. The AOE is a circle around your ship. Also removes all fuel from missiles within the radius.
    // Cooldown: 120s

    [Header("Ability Settings")]
    [SerializeField] private float _abilityCooldown = 120f;
    [SerializeField] private float _abilityDuration = 4f;
    [SerializeField] private float _solarFlareRadius = 5f; // Play around with values
    [SerializeField] private GameObject _solarFlare;
    private float abilityTimer;

    protected override void ManagementStart()
    {
        Debug.Log("Star Child is angry!");

        // TODO:
        // Select a random crew member
        // Make that crew member travel to Star Child
        // Require 8 seconds of interaction to soothe it.
    }

    protected override void EnableBuffs()
    {
        if (IsStressed)
            return;
        switch (AssignedRoom)
        {
            case AnomalyRoom.Shield:
                ShieldRoom();
                break;
            case AnomalyRoom.Engine:
                EngineRoom();
                break;
            case AnomalyRoom.Weapons:
                WeaponRoom();
                break;
            case AnomalyRoom.Lens:
                Debug.Log("Solar Flare ability available.");
                break;
        }
        Debug.Log("Star Child buffs enabled.");
    }

    protected override void DisableBuffs()
    {
        Debug.Log("Star Child buffs disabled");
        switch (AssignedRoom)
        {
            case AnomalyRoom.Shield:
                // TODO: Remove additional shield block
                break;
            case AnomalyRoom.Engine:
                // TODO: Restore normal engine speed
                break;
            case AnomalyRoom.Weapons:
                // TODO: Restore normal beam length
                break;
            case AnomalyRoom.Lens:
                // No passive buff to remove
                break;
        }
    }


    private void ShieldRoom()
    {
        // TODO: Allow shield to block one additional shot
    }

    private void EngineRoom()
    {
        // TODO: Double engine speed
    }

    private void WeaponRoom()
    {
        // TODO: Increase beam length by 1.5x
    }

    // Called by ability button
    public void ActivateSolarFlare()
    {
        if (AssignedRoom != AnomalyRoom.Lens || IsStressed)
            return;

        if (Time.time < abilityTimer)
        {
            Debug.Log("Solar Flare is on cooldown");
            return;
        }

        // Start cooldown immediately upon activation
        abilityTimer = Time.time + _abilityCooldown;
        Debug.Log("Solar Flare activated!");

        // TODO:
        // Find enemy systems within the radius
        // Disable them for 4 seconds
        // Remove fuel from missiles within the radius
    }

    // Called when a battle begins
    public override void OnCombatStarted()
    {
        abilityTimer = Time.time + (_abilityCooldown * 0.5f);
        Debug.Log("Star Child cooldown initialized");
    }

    public float GetRemainingCooldown()
    {
        return Mathf.Max(0f, abilityTimer - Time.time);
    }
}
