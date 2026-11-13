using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsTemplate : SystemsTemplate
{
    [SerializeField] protected bool isSelected = false;
    [SerializeField] protected InputActionReference fireAction;

    [SerializeField] protected GameObject barrel;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Transform weaponRadius;
    [SerializeField] protected Transform projectileSpawn;
    [SerializeField] protected float maxRange = 20f;
    
    [SerializeField] protected float maxRecharge = 20f;
    [SerializeField] protected float currentRecharge = 0f;

    [SerializeField] protected float barrelSpeed = 0.5f;

    // [SerializeField] protected bool isRotLocked = false;

    [SerializeField] protected GameObject reticle;
    protected Vector3 lastMousePos;

    protected bool isReady = false;

    private Camera cam;

    protected void Awake()
    {
        reticle.SetActive(true);
        cam = Camera.main;
        SetPreferredCrew(1);

        // sets the plugged crew to gunner; for TESTING ONLY
        SetCrew(preferredCrew);
        isSelected = false;

        weaponRadius.localScale = new Vector3(maxRange, maxRange, 0f);

        fireAction.action.performed += OnClick;
        fireAction.action.Enable();
        // lastMousePos = projectileSpawn.transform.position + Vector3.right * maxRange;
        lastMousePos = projectileSpawn.transform.position + Vector3.right * maxRange * 0.5f;
    }

    protected void Update()
    {
        BarrelLookAtMouse();
        reticle.transform.position = lastMousePos;
        // if(!isRotLocked)
        // {
        //     // BarrelLook(lastMousePos);
        //     BarrelLookAtMouse();
        // }
        // BarrelLook(lastMousePos);
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
            Debug.Log("Weapon is ready!");
        }
    }


    // protected void Attack()
    // {
    //     // Debug.Log("FIRE!!!");
    //     SetCurrentRecharge(0);
    //     SetIsReady(false);
    // }

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


            // BarrelLook(mousePos);
        }
        
        BarrelLook(lastMousePos);
    }

    protected void BarrelLook(Vector3 target)
    {
        // if (isRotLocked)
        // {
        //     Debug.Log("weapon locked!");
        //     return;
        // }
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
        isSelected = !isSelected;
    }
    
    public virtual void FireWeapon()
    {
        Debug.Log("I don't know how to fire...");
    }
}
