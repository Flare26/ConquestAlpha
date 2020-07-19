
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform bulletSpawn;
    public float speed = 70f;// Chosen default
    public float TTL = 3f; // time to live in sec
    Transform targetTransform;
    public int dmg;
    
    public GameObject impactEffect;
    Rigidbody m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            //Debug.Log("Congrats you shot a wall...");
            DestroyBullet();
            return;
        }
        if (collision.gameObject.tag == "Floor")
        {
            //Debug.Log("Congrats you shot the floor...");
            DestroyBullet();
            return;
        }
            
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            //Debug.Log("Congrats you shot a wall...");
            DestroyBullet();
            return;
        }
        if (collision.gameObject.tag == "Floor")
        {
            //Debug.Log("Congrats you shot the floor...");
            DestroyBullet();
            return;
        }

    }


    public void HitTarget()
    {
        //Debug.Log("A bullet hit it's target!");
        DestroyBullet();
    }

    private void DestroyBullet()
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
