using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsTemplate : SystemsTemplate
{
    protected bool isSelected = false;
    public GameObject barrel;
    public GameObject projectile;
    
    [SerializeField]protected float maxRecharge;
    [SerializeField]protected float currentRecharge;

    [SerializeField]protected float barrelSpeed = 0.5f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SetPreferredCrew(1);
    }

    void Update()
    {
        BarrelLookAtMouse();
        
        Debug.Log("looking at mouse");
    }

    public void SetCurrentRecharge(float newRecharge)
    {
        currentRecharge = newRecharge;
    }

    public float GetCurrentRecharge()
    {
        return currentRecharge;
    }

    public void BarrelLookAtMouse()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Mathf.Abs(cam.transform.position.z - barrel.transform.position.z);
        mousePos = cam.ScreenToWorldPoint(mousePos);
        Vector2 dir = mousePos - barrel.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // barrel.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        
        barrel.transform.rotation = Quaternion.Slerp(barrel.transform.rotation, 
            Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * barrelSpeed);
    }

    
}
