using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Beam : WeaponsTemplate
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<GameObject> hitList;

    [SerializeField] private float beamDuration;
    // [SerializeField] private float beamLength;
    // [SerializeField] private GameObject beamBullet;

    // Update is called once per frame

    private void Awake()
    {
        // defaults beam to disabled
        base.Awake();
        projectile.SetActive(false);

        // sets the length of the beam
        projectile.transform.localScale = new Vector3(maxRange, projectile.transform.localScale.y, 0);
    }
    public override void FireWeapon()
    {
        StartCoroutine(BeamFire());
    }

    public IEnumerator BeamFire()
    {
        SetIsFiring(true);
        projectile.SetActive(true);

        // waits for {beamDuration} seconds before turning the beam off
        yield return new WaitForSeconds(beamDuration);
        projectile.SetActive(false);
        SetIsFiring(false);
        SetCurrentRecharge(0);
    }


}
