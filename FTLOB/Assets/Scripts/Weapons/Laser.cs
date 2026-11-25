using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Laser : WeaponsTemplate
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private int bulletNum;
    [SerializeField] private float bulletDelay;
    [SerializeField] private float bulletSpeed;
    // [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpread;

    [SerializeField] private Coroutine activeCorountine;

    // Update is called once per frame

    public bool isCoroutineActive()
    {
        return activeCorountine != null;
    }

    public override void FireWeapon()
    {
        if(isCoroutineActive())
        {
            Debug.Log("I can't fire right now!");
            return;
        }
        // SetIsFiring(true);
        // SetIsReady(false);
        activeCorountine = StartCoroutine(BurstFire());
    }
    public IEnumerator BurstFire()
    {

        SetIsFiring(true);
        // if(!isFiring){
        if(isCoroutineActive())
        {
            StopCoroutine(activeCorountine);
        }
        Debug.Log("FIRIN MA LAYSA!!!");
        for(int i = 0; i < bulletNum; i++)
        {
            // SetIsFiring(true);
            //instantiate laser
            Debug.Log("fired 1 laser!");
            GameObject newBullet = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
            newBullet.transform.Rotate(new Vector3(0, 0, Random.Range(-1 * bulletSpread, bulletSpread)));
            newBullet.GetComponent<LaserBullet>().SetEnemyTag(enemyTag);
            newBullet.GetComponent<LaserBullet>().SetBulletSpeed(bulletSpeed);

            // instantiates a new weapon radius, which the bullet will use to check if it is at the maximum range
            // currently, the actual maximum range is slightly longer than the specified range
            // this is because the bullet is deleted when it leaves the radius' collider, and does not actually check distance
            GameObject newRadius = Instantiate(weaponRadius.gameObject, weaponRadius.position, weaponRadius.rotation);

            // // disables sprite for the current bullet's radius
            newRadius.GetComponent<SpriteRenderer>().enabled = false;
            newRadius.GetComponent<CircleCollider2D>().enabled = true;

            // enables visibility of max bullet distance, CHANGE TO TRANSPARENT LATER
            // newRadius.GetComponent<SpriteRenderer>().color = Color.black; 

            newRadius.SetActive(true);
            newBullet.GetComponent<LaserBullet>().SetBulletRadius(newRadius);
            newBullet.SetActive(true);
            
            // wait for {bulletDelay} amount of seconds, then fire again
            yield return new WaitForSeconds(bulletDelay);
        }
        SetIsFiring(false);
        SetCurrentRecharge(0);
        StopCoroutine(activeCorountine);
        activeCorountine = null;
    // }
    }

}
