
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform bulletSpawn;
    public float speed = 70f;
    public float TTL = 3f; // time to live in sec
    Transform targetTransform;
    
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
    void HitTarget()
    {
        
        GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 0.5f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Rigidbody.velocity = transform.forward * speed;

        if (Vector3.Distance(targetTransform.position, transform.position) <= speed * Time.deltaTime)
            HitTarget();

        if (TTL > 0)
            TTL -= Time.deltaTime;
        else
            Destroy(gameObject); 
    }
    
}
