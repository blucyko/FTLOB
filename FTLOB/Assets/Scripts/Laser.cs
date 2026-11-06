using UnityEngine;
using System.Collections;

public class Laser : WeaponsTemplate
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private int bulletNum;
    [SerializeField] private float bulletDelay;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bullet;

    // Update is called once per frame
    public void FireLaser()
    {
        StartCoroutine(Attack());
    }
    public IEnumerator Attack()
    {
        if(isReady)
        {
            Debug.Log("FIRIN MA LAYSA!!!");
            for(int i = 0; i < bulletNum; i++)
            {
                //TODO: instantiate laser
                Debug.Log("fired 1 laser!");
            GameObject newBullet = Instantiate(bullet, weaponRadius.position, weaponRadius.rotation);
            newBullet.GetComponent<LaserBullet>().SetBulletSpeed(bulletSpeed);
            GameObject newRadius = Instantiate(weaponRadius.gameObject, weaponRadius.position, weaponRadius.rotation);
            // newRadius.GetComponent<SpriteRenderer>().enabled = false;
            newRadius.GetComponent<SpriteRenderer>().color = Color.black;
            newRadius.SetActive(true);
            newBullet.GetComponent<LaserBullet>().SetBulletRadius(newRadius);
            newBullet.SetActive(true);
                yield return new WaitForSeconds(bulletDelay);
            }
            base.Attack();
        }
        else
        {
            Debug.Log("laser isn't ready yet!");
        }
    }
}
