using System.Collections.Generic;
using UnityEngine;

public class EnemyTemplate : MonoBehaviour
{
    [SerializeField] protected List<WeaponsTemplate> weaponsList;
    [SerializeField] protected List<bool> weaponReady;
    [SerializeField] protected Health health;

    [SerializeField] protected Laser laser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        // for(int i = 0; i < weaponsList.Count; i++)
        // {
        //     weaponReady.Add(false);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // for(int i = 0; i < weaponsList.Count; i++)
        // {
        //     if(weaponsList[i].GetIsReady() && !weaponsList[i].GetIsFiring())
        //     {
        //         weaponReady[i] = true;
        //     }
        //     else
        //     {
        //         weaponReady[i] = false;
        //     }
        //     if(weaponReady[i])
        //     {
        //         weaponsList[i].FireWeapon();
        //     }
        // }

        if(laser.GetIsReady() && !laser.GetIsFiring() && !laser.isCoroutineActive())
        {
            laser.FireWeapon();
        }
    }
}
