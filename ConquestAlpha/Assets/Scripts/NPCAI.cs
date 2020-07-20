using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



// these are going to be created and built with the team of the base
public class NPCAI : MonoBehaviour
{
    
    [SerializeField] Transform destination;
    NavMeshAgent navAgent;
    [SerializeField] public Team m_Team;
    [SerializeField] GameObject shieldPopFX;
    [SerializeField] GameObject deathPopFX;
    string team;
    public int hull = 10;
    public int shield = 5;
    public int maxShield = 5;
    public float shieldDowntime = 3f;
    bool hasShield = true;
    private GameObject shieldFX;
    private GameObject deathFX;
    float safeTime = 0f;
    public float range = 7f;
    
    // Start is called before the first frame update

    void Start()
    {

        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.Log("NAV ERROR: Please attach NavMeshAgent to " + gameObject.name);
        }
        else
            SetDestination();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(collision.gameObject.GetComponent<Bullet>()); // Get Bullet script instance to see what type of bullet has hit us
            collision.gameObject.GetComponent<Bullet>().HitTarget();
        }
    }

    void TakeDamage(Bullet b)
    {
        //Destroy(gameObject);
        
        if (hasShield == false) {
            hull -= b.dmg;
        }
            
        if (b.dmg >= shield && hasShield) // if dmg is greater than current shield
        {
            //Debug.Log("Shield Pop!!");
            hasShield = false;
            shieldFX = (GameObject)Instantiate(shieldPopFX, transform.position, transform.rotation);
            Destroy(shieldFX, 3f);
            hull -= (b.dmg - shield); // use the remaining shield to mitigate
            shield = 0;
        } else if (b.dmg < shield)
        {
            shield -= b.dmg;
            return;
        }

        //Debug.Log(gameObject.name + " took dmg " + b.dmg);
    }

    void TakeDamage(int dmg)
    {
        int d = shield - dmg;
        if (d < 0) // if dmg is greater than current shield
            hull -= dmg;
    }

    private void SetDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navAgent.SetDestination(targetVector);
        }
    }

    void RegenShield()
    {
        float chargeRate = 5;
        if ( shield < maxShield )
           shield += (int) ( Time.deltaTime * chargeRate * 100 ) ;
    }
    private void FixedUpdate()
    {   

        if (shieldFX != null)
        {
            shieldFX.transform.position = this.transform.position;
        }

        if (hull <= 0)
        {
            deathFX = (GameObject)Instantiate(deathPopFX, transform.position, transform.rotation);
            Destroy(shieldFX, 4f);
            Destroy(deathFX, 4f);
            Destroy(gameObject);
        }
    }
}
