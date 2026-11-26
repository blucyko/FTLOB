using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponCharge : MonoBehaviour
{
    public enum WeaponState
    {
        Ready, Charging, Target
    }

    [Header("UI References")]
    [SerializeField] private TMP_Text weaponStateText;
    [SerializeField] private Image chargingBar;

    [Header("Charge Settings")]
    [SerializeField] private float chargeDuration = 5f; // modify based on actual charging duration in GDD and weapons template

    [Header("Weapon Colors")]
    [SerializeField] private Color readyColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Color chargingColor = new Color32(227, 165, 72, 255);
    [SerializeField] private Color targetColor = new Color32(95, 198, 90, 255);

    private WeaponState currentState;

    private float chargeProgress;
    private float chargeTimer;

    public WeaponState CurrentState => currentState;
    public float ChargeProgress => chargeProgress;

    private void Start()
    {
        if (chargeDuration <= 0f)
        {
            Debug.LogError($"{gameObject.name}: Charge duration must be greater than zero!");
            enabled = false;
            return;
        }

        SetState(WeaponState.Ready);

        Debug.Log($"{gameObject.name}: Weapon initialized and ready.");
    }

    private void Update()
    {
        if (currentState == WeaponState.Charging)
        {
            ChargeWeapon();
        }
    }

    // Handles the weapon charging over time
    private void ChargeWeapon()
    {
        chargeTimer += Time.deltaTime;

        chargeProgress = Mathf.Clamp01(chargeTimer / chargeDuration);

        chargingBar.fillAmount = chargeProgress;

        // Weapon has finished charging
        if (chargeProgress >= 1f)
        {
            SetState(WeaponState.Target);

            Debug.Log($"{gameObject.name}: Charging complete! Weapon ready to target.");
        }
    }

    // Called when the weapon begins charging
    public void BeginCharging()
    {
        if (currentState != WeaponState.Ready)
        {
            Debug.LogWarning($"{gameObject.name}: Cannot charge. Current state: {currentState}");
            return;
        }

        chargeTimer = 0f;
        chargeProgress = 0f;

        SetState(WeaponState.Charging);

        Debug.Log($"{gameObject.name}: Weapon charging started.");
    }

    // Updates the UI based on the current weapon state
    private void SetState(WeaponState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case WeaponState.Ready:
                weaponStateText.text = "READY";
                chargingBar.color = readyColor;
                chargingBar.fillAmount = 1f;
                chargeProgress = 1f;
                break;

            case WeaponState.Charging:
                weaponStateText.text = "CHARGING";
                chargingBar.color = chargingColor;
                chargingBar.fillAmount = 0f;
                break;

            case WeaponState.Target:
                weaponStateText.text = "TARGET";
                chargingBar.color = targetColor;
                chargingBar.fillAmount = 1f;
                chargeProgress = 1f;
                break;
        }
    }

    // Called through the Button's On Click() event
    public void OnWeaponClicked()
    {
        Debug.Log($"{gameObject.name}: Weapon button clicked! Current state: {currentState}");

        if (currentState == WeaponState.Ready)
        {
            BeginCharging();
        }
        else if (currentState == WeaponState.Target)
        {
            Debug.Log($"{gameObject.name}: Weapon selected! Ready to target an enemy room.");
        }
        else if (currentState == WeaponState.Charging)
        {
            Debug.Log($"{gameObject.name}: Weapon is still charging. Cannot select yet.");
        }
    }

    // Called after the weapon has fired and is ready to charge again
    public void ResetWeapon()
    {
        chargeTimer = 0f;
        chargeProgress = 0f;

        SetState(WeaponState.Ready);

        Debug.Log($"{gameObject.name}: Weapon reset and ready to charge again.");
    }
}