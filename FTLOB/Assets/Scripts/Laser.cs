using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Laser : WeaponsTemplate
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private int bulletNum;
    [SerializeField] private float bulletDelay;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpread;

    // Update is called once per frame

    private void Awake()
    {
        base.Awake();
        
        fireAction.action.performed += OnClick;
        fireAction.action.Enable();
    }
    public override void FireWeapon()
    {
        StartCoroutine(Attack());
    }
    public IEnumerator Attack()
    {
        if(isReady)
        {
            // SetIsRotLocked(true);
            Debug.Log("FIRIN MA LAYSA!!!");
            for(int i = 0; i < bulletNum; i++)
            {
                //TODO: instantiate laser
                Debug.Log("fired 1 laser!");
            GameObject newBullet = Instantiate(bullet, projectileSpawn.position, projectileSpawn.rotation);
            newBullet.transform.Rotate(new Vector3(0, 0, Random.Range(-1 * bulletSpread, bulletSpread)));
            newBullet.GetComponent<LaserBullet>().SetBulletSpeed(bulletSpeed);
            GameObject newRadius = Instantiate(weaponRadius.gameObject, weaponRadius.position, weaponRadius.rotation);
            // newRadius.GetComponent<SpriteRenderer>().enabled = false;

            // enables visibility of max bullet distance, CHANGE TO TRANSPARENT LATER
            newRadius.GetComponent<SpriteRenderer>().color = Color.black; 

            newRadius.SetActive(true);
            newBullet.GetComponent<LaserBullet>().SetBulletRadius(newRadius);
            newBullet.SetActive(true);
                yield return new WaitForSeconds(bulletDelay);
            }
            base.Attack();
            // SetIsRotLocked(false);
            ToggleIsSelected();
        }
        else
        {
            Debug.Log("laser isn't ready yet!");
        }
    }

    // void OnClick(InputAction.CallbackContext context)
    // {
    //     if(isSelected)
    //     {
    //         // fire the thing!
    //         Debug.Log("trigger fire");
    //         FireLaser();
    //     }
        
    // }
}
