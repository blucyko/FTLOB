using System;
using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float bulletSpeed = 0;
    // [SerializeField] private float maxRange = 20;
    [SerializeField] private GameObject bulletRadius;
    private Rigidbody2D rb;

    void Start()
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

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("bullet exited radius!");
        Debug.Log("tag of radius is: " + collision.gameObject.tag);
        // if (collision.gameObject.tag.Equals("weapon radius"))
        if (collision.gameObject == bulletRadius)
        {
            Debug.Log("destroy!");
            Destroy(bulletRadius);
            Destroy(this.gameObject);
        }
    }
}
