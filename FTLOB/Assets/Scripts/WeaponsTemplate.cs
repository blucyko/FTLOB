using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsTemplate : SystemsTemplate
{
    protected bool isSelected = false;
    public GameObject barrel;
    public GameObject projectile;
    
    [SerializeField]protected float maxRecharge = 20;
    [SerializeField]protected float currentRecharge = 0;

    [SerializeField]protected float barrelSpeed = 0.5f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SetPreferredCrew(1);
        SetCrew(pluggedCrew);
    }

    void Update()
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
    }

    protected void Attack()
    {
        // Debug.Log("FIRE!!!");
        SetCurrentRecharge(0);
    }

    public void SetCurrentRecharge(float newRecharge)
    {
        currentRecharge = newRecharge;
    }

    public float GetCurrentRecharge()
    {
        return currentRecharge;
    }

    protected void BarrelLookAtMouse()
    {
        // mouse position calculation
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Mathf.Abs(cam.transform.position.z - barrel.transform.position.z);
        mousePos = cam.ScreenToWorldPoint(mousePos);

        // determining required angle to look at mouse
        Vector2 dir = mousePos - barrel.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // barrel.transform.rotation = Quaternion.Euler(0f, 0f, angle); // desired final rotation

        
        barrel.transform.rotation = Quaternion.Slerp(barrel.transform.rotation, 
            Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * barrelSpeed); // rotation accounting for speed
    }

    
}
