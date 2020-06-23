
using UnityEngine;

public class NewBullet : MonoBehaviour
{
    public Transform bulletSpawn;
    public float speed = 70f;
    public float TTL = 3f; // time to live in sec
    Transform target;
    
    public GameObject impactEffect;
    Rigidbody m_Rigidbody;

    private void Start()
    {

        m_Rigidbody = GetComponent<Rigidbody>();
    }
    void SetTarget(Transform target)
    {
        this.target = target;
    }
    void HitTarget()
    {
        
        GameObject effectInstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 0.5f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        m_Rigidbody.velocity = transform.forward * speed;

        if (TTL > 0)
            TTL -= Time.deltaTime;
        else
            Destroy(gameObject); 
    }
    
}
