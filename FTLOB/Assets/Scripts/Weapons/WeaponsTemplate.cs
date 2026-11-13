using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsTemplate : SystemsTemplate
{
    /// <summary>
    /// <para>true if the weapon is currently selected </para>
    /// </summary>
    [SerializeField] protected bool isSelected = false;

    /// <summary>
    /// <para>true if the weapon is currently firing </para>
    /// </summary>
    [SerializeField]protected bool isFiring = false;

    /// <summary>
    /// <para>input for firing the weapon </para>
    /// </summary>
    [SerializeField] protected InputActionReference fireAction;

    /// <summary>
    /// <para>GameObject for the weapon's barrel </para>
    /// </summary>
    [SerializeField] protected GameObject barrel;

    /// <summary>
    /// <para>GameObject of the weapon's projectile </para>
    /// </summary>
    [SerializeField] protected GameObject projectile;

    /// <summary>
    /// <para>GameObject for the weapon's max range </para>
    /// </summary>
    [SerializeField] protected Transform weaponRadius;

    /// <summary>
    /// <para>Transform where the projectile is spawned at </para>
    /// </summary>
    [SerializeField] protected Transform projectileSpawn;
    
    /// <summary>
    /// <para>size of the weapon's radius </para>
    /// </summary>
    [SerializeField] protected float maxRange = 20f;
    
    /// <summary>
    /// <para>time in seconds it takes for the weapon to recharge with the preferred crew memeber </para>
    /// </summary>
    [SerializeField] protected float maxRecharge = 20f;
    
    /// <summary>
    /// <para>current recharge of the weapon (updated every frame until it hits max recharge) </para>
    /// </summary>
    [SerializeField] protected float currentRecharge = 0f;

    /// <summary>
    /// <para>speed at which the barrel turns to look at the target </para>
    /// </summary>
    [SerializeField] protected float barrelSpeed = 0.5f;

    /// <summary>
    /// <para>reticle for the weapon (i.e. target that the barrel will look at) </para>
    /// </summary>
    [SerializeField] protected GameObject reticle;
    
    /// <summary>
    /// <para>position that the barrel will look at </para>
    /// </summary>
    protected Vector3 lastMousePos;

    /// <summary>
    /// <para>true if the weapon is fully charged </para>
    /// </summary>
    protected bool isReady = false;

    /// <summary>
    /// <para>main camera of the scene </para>
    /// </summary>
    private Camera cam;

    protected void Awake()
    {
        reticle.SetActive(true);
        cam = Camera.main;
        SetPreferredCrew(1);

        // sets the plugged crew to gunner; for TESTING ONLY
        SetCrew(preferredCrew);
        isSelected = false;

        //sets the scale of the weapon radius based on the weapon's maximum range
        weaponRadius.localScale = new Vector3(maxRange, maxRange, 0f);

        // links the fire input to the function that fires the weapon
        fireAction.action.performed += OnClick;
        fireAction.action.Enable();

        // initiates the starting location for the reticle
        lastMousePos = projectileSpawn.transform.position + Vector3.right * maxRange * 0.5f;

        SetIsFiring(false);
    }

    protected void Update()
    {
        // every frame, tries to make the barrel look at the target position
        BarrelLookAtMouse();

        // changes the position of the reticle to the target position
        reticle.transform.position = lastMousePos;
        UpdateRecharge();
    }

    protected void UpdateRecharge()
    {
        if(currentRecharge < maxRecharge)
        {
            SetCurrentRecharge(currentRecharge + Time.deltaTime * crewMod);
        }
        if(currentRecharge >= maxRecharge)
        {
            SetIsReady(true);
            // Debug.Log("Weapon is ready!");
        }
    }

    public void SetCurrentRecharge(float newRecharge)
    {
        currentRecharge = newRecharge;
    }

    public float GetCurrentRecharge()
    {
        return currentRecharge;
    }

    public void SetIsReady(bool newIsReady)
    {
        isReady = newIsReady;
    }
    public bool GetIsReady()
    {
        return isReady;
    }

    protected void BarrelLookAtMouse()
    {
        if(isSelected)
        {
            // mouse position calculation
            lastMousePos = Mouse.current.position.ReadValue();
            lastMousePos.z = Mathf.Abs(cam.transform.position.z - barrel.transform.position.z);
            lastMousePos = cam.ScreenToWorldPoint(lastMousePos);
        }
        
        BarrelLook(lastMousePos);
    }

    protected void BarrelLook(Vector3 target)
    {
        // determining required angle to look at mouse
        Vector2 dir = target - barrel.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // barrel.transform.rotation = Quaternion.Euler(0f, 0f, angle); // desired final rotation

        
        barrel.transform.rotation = Quaternion.Slerp(barrel.transform.rotation, 
            Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * barrelSpeed); // rotation accounting for speed

    }


    public void SetLastMousePos(Vector3 newMousePos)
    {
        lastMousePos = newMousePos;
    }


    /// <summary>
    /// <para>when the weapon is selected, fires the weapon if the recharge is full, otherwise do nothing </para>
    /// <para>in all cases, unselects the weapon when called </para>
    /// </summary>
    protected void OnClick(InputAction.CallbackContext context)
    {
        if(isSelected)
        {
            // fire the thing!
            Debug.Log("trigger fire");
            ToggleIsSelected();
            
            if(!isReady)
            {
                Debug.Log("Weapon not ready!");
                return;
            }
            
            FireWeapon();
            SetCurrentRecharge(0);
            SetIsReady(false);
        }
        
    }

    public void ToggleIsSelected()
    {
        if(!isFiring)
        {
            isSelected = !isSelected;
        }
    }
    
    public void SetIsFiring(bool newIsFire)
    {
        isFiring = newIsFire;
    }
    
    public virtual void FireWeapon()
    {
        Debug.Log("I don't know how to fire...");
    }
}
