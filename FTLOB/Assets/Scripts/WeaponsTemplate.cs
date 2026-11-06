using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsTemplate : SystemsTemplate
{
    [SerializeField] protected bool isSelected = false;
    [SerializeField] protected InputActionReference selectAction;
    [SerializeField] protected InputActionReference pointAction;

    [SerializeField] protected GameObject barrel;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Transform weaponRadius;
    [SerializeField] protected float maxRange = 20f;
    
    [SerializeField] protected float maxRecharge = 20f;
    [SerializeField] protected float currentRecharge = 0f;

    [SerializeField] protected float barrelSpeed = 0.5f;

    [SerializeField] protected bool isRotLocked = false;

    protected bool isReady = false;

    private Camera cam;

    protected void Awake()
    {
        cam = Camera.main;
        SetPreferredCrew(1);
        SetCrew(pluggedCrew);

        weaponRadius.localScale = new Vector3(maxRange, maxRange, 0f);

        selectAction.action.performed += OnClick;
        selectAction.action.Enable();
    }

    protected void Update()
    {
        BarrelLookAtMouse();
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


    protected void Attack()
    {
        // Debug.Log("FIRE!!!");
        SetCurrentRecharge(0);
        SetIsReady(false);
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
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = Mathf.Abs(cam.transform.position.z - barrel.transform.position.z);
            mousePos = cam.ScreenToWorldPoint(mousePos);

            BarrelLook(mousePos);
        }
    }

    protected void BarrelLook(Vector3 target)
    {
        if (isRotLocked)
        {
            Debug.Log("weapon locked!");
            return;
        }
        // determining required angle to look at mouse
        Vector2 dir = target - barrel.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // barrel.transform.rotation = Quaternion.Euler(0f, 0f, angle); // desired final rotation

        
        barrel.transform.rotation = Quaternion.Slerp(barrel.transform.rotation, 
            Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * barrelSpeed); // rotation accounting for speed

    }

    void OnClick(InputAction.CallbackContext context)
    {
        if(!context.started)
        {
            Debug.Log("couldn't find context!");
            return;
        }

        var rayHit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if(!rayHit.collider)
        {
            Debug.Log("didn't hit!");
        }

        Debug.Log("Hit something: " + rayHit.collider.gameObject.name);
    }

    public void SetIsRotLocked(bool newLock)
    {
        isRotLocked = newLock;
    }
}
