using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Xml.Serialization;
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
    TargetingAgent tgtAgent;
    Transform tgt_Transform;
    Transform mount_Primary;
    Transform mount_Secondary;
    public Transform p_FP; // primary fire point
    public Transform s_FP; // secondary fire point
    public GameObject primaryWep;
    public GameObject secondaryWep;
    public string targetName;
    public int hull = 10;
    public int shield = 5;
    public int magSize = 3;
    private int roundCount = 5;
    public int maxShield = 5;
    public float shieldDowntime = 3f;
    bool hasShield = true;
    private GameObject shieldFX;
    private GameObject deathFX;
    float safeTime = 0f;
    public float range = 20f;
    GameObject primaryInstance;
    private float tm = 0f;
    public float rldTime = 1; // reload time in seconds
    public float rrld; // remainder reload time
    public float fireRate;
    private void OnEnable()
    {
        
        mount_Primary = transform.GetChild(0).transform;
        mount_Secondary = transform.GetChild(1).transform;

        if (primaryWep != null)
        {
            primaryInstance = Instantiate<GameObject>(primaryWep, mount_Primary.position, this.transform.rotation);
        }
            // Secondary wep not accounted for yet

        // in the prefab the mount points should be first 2 children on the npc
        // The target sphere should always be the last
        var targetSphere = transform.GetChild(transform.childCount-1);
        tgtAgent = targetSphere.GetComponent<TargetingAgent>();

        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.Log("NAV ERROR: Please attach NavMeshAgent to " + gameObject.name);
        }
        else
            SetDestination();
        InvokeRepeating("UpdateTarget", 0, 0.42f);
    }
    void UpdateTarget()
    {
        tgt_Transform = tgtAgent.RequestClosestTarget();
        if (tgt_Transform == null)
        {
            targetName = "Out of Range";
        }
        else
        {
            targetName = tgt_Transform.name;
        }
    }
    void FirePrimary()
    {
        if (primaryWep != null)
        {
            primaryWep.GetComponent<WeaponCore>().Fire(p_FP.position, p_FP.transform.rotation); // The angle of fire can be adjusted via firepoint
        }
    }

    void FireSecondary()
    {

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

    public void Update()
    {
        //Vector3 dir = tgt_Transform.position - transform.position;

        if (!targetName.Equals("Out of Range"))
        {
            transform.LookAt(tgt_Transform);
            if (roundCount != 0) // if there are not 0 rounds in the mag then try to fire
            {
                if (tm <= 0f)
                {
                    FirePrimary();
                    roundCount -= 1;
                    tm = fireRate;
                }
            }


        }

        if (roundCount == 0)
        {
            if (rrld == 0)
                rrld = rldTime; // re set the remaining reload time counter if it's currently 0 from the last reload....
            if(rrld > 0)
            {
                rrld -= Time.deltaTime;
                if (rrld < 0)
                {
                    roundCount = magSize; // if this iteration causes the counter to reach 0, the reload is complete and round count should be refreshed
                    rrld = 0; // The engine is not perfect at detecting the instant it reaches 0 :( 
                }
            }
        }

        tm -= Time.deltaTime; // fire rate will decrement regardless of having a target
    }

    private void FixedUpdate()
    {
        if (primaryInstance != null)
        {
            primaryInstance.transform.position = p_FP.position;
            primaryInstance.transform.rotation = p_FP.rotation;
        }
            
        if (shieldFX != null)
        {
            shieldFX.transform.position = this.transform.position;
        }

        if (hull <= 0)
        {
            deathFX = (GameObject)Instantiate(deathPopFX, transform.position, transform.rotation);
            Destroy(shieldFX, 4f);
            Destroy(deathFX, 4f);
            Destroy(primaryInstance);
            //Destroy(secondaryInstance);
            Destroy(gameObject);
        }
    }
}
