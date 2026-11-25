using System;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float bulletSpeed = 0;
    // [SerializeField] private float maxRange = 20;
    [SerializeField] private GameObject bulletRadius;
    [SerializeField] private string enemyTag = "Enemy";
    private Rigidbody2D rb;

    void OnEnable()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
    }

    public void SetBulletSpeed(float newSpeed)
    {
        bulletSpeed = newSpeed;
    }

    public void SetBulletRadius(GameObject newRadius)
    {
        bulletRadius = newRadius;
    }

    public void SetEnemyTag(string newTag)
    {
        enemyTag = newTag;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Debug.Log("bullet exited radius!");
        // Debug.Log("tag of radius is: " + collision.gameObject.tag);
        // if (collision.gameObject.tag.Equals("weapon radius"))
        if (collision.gameObject == bulletRadius)
        {
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Debug.Log("destroy!");
        Destroy(bulletRadius);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        RoomHealth damagedRoom = collision.gameObject.GetComponent<RoomHealth>();
        if(collision.gameObject.tag == enemyTag && damagedRoom != null)
        {
            // Debug.Log("it's an enemy!");
            damagedRoom.Damage(1);
            Destroy(gameObject);
        }
    }
}
