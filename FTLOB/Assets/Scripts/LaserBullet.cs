using System;
using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float bulletSpeed = 0;
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

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("bullet exited radius!");
        Debug.Log("tag of radius is: " + collision.gameObject.tag);
        if (collision.gameObject.tag.Equals("weapon radius"))
        {
            Debug.Log("destroy!");
            Destroy(this.gameObject);
        }
    }
}
