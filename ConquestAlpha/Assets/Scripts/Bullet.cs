
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform bulletSpawn;
    public float speed = 70f;// Chosen default
    public float TTL = 3f; // time to live in sec
    
    public int m_dmg;
    
    public GameObject impactEffect;
    Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(9, 9);
    }
    void OnCollisionEnter(Collision collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("Wall"))
        {
            //Debug.Log("Congrats you shot a wall...");
            DestroyBullet();
            return;
        }
        if (other.CompareTag("Floor"))
        {
            //Debug.Log("Congrats you shot the floor...");
            DestroyBullet();
            return;
        }
        if (other.CompareTag("Bullet"))
        {
            DestroyBullet();
            return;
        }
    }


    public void HitTarget()
    {
        //Debug.Log("A bullet hit it's target!");
        DestroyBullet();
    }

    public void DestroyBullet()
    {
        // Call Hit Target, not Destroy bullet please! :)
        GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 0.5f);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        m_Rigidbody.velocity = transform.forward * speed;

        //if (Vector3.Distance(targetTransform.position, transform.position) <= speed * Time.deltaTime)
           // HitTarget();

        if (TTL > 0)
            TTL -= Time.deltaTime;
        else
            Destroy(gameObject); 
    }
    
}
