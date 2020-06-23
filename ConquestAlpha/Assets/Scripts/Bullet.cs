
using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Pass target from turret to bullet
    // Start is called before the first frame update
    
    private Transform target;
    public float speed = 70f;

    public GameObject impactEffect;
    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    void HitTarget()
    {
        //Debug.Log("weve been hit!");
        GameObject effectInstance = (GameObject) Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInstance, 2f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World); // move the bullet relative to world space

    }
}
