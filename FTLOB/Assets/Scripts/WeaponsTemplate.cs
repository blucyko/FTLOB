using System;
using UnityEngine;

public class WeaponsTemplate : SystemsTemplate
{
    private bool isSelected = false;
    public GameObject barrel;
    public GameObject projectile;
    
    public float maxRecharge;
    private float currentRecharge;

    void Start()
    {
        SetPreferredCrew(1);
    }

    public void SetCurrentRecharge(float newRecharge)
    {
        currentRecharge = newRecharge;
    }

    public float GetCurrentRecharge()
    {
        return currentRecharge;
    }
}
