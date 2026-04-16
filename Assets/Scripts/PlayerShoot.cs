using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //left click
        {
            Shoot();
        }
    }

    void Shoot()
    {
        //get mouse position 
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Direction from us to mouse
        Vector3 shootDirection = (mousePosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(shootDirection.x, shootDirection.y) * bulletSpeed;
        Destroy(bullet, 2f);
    }
}
