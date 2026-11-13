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
        base.Awake();
        projectile.SetActive(false);
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
        yield return new WaitForSeconds(beamDuration);
        projectile.SetActive(false);
        SetIsFiring(false);
    }


}
