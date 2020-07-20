using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [SerializeField] public Team m_Team;
    [SerializeField] public Team e_Team;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnTransform; // use an empty game object as reference for fire point so bullet does not spawn within turret
    [SerializeField] GameObject tHead;
    
    public string targetName = "No Target";
    public int dmg_Mod;
    TargetingAgent tgtAgent;
    private float tm = 0f; // Time since last shot
    public float turnSpeed; // clamped 0-1
    public float range;
    public float fireRate; // shots per sec

    Transform tgt_Transform;
    public float shotVelocityMult;
    
    // Start is called before the first frame update
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(tHead.transform.position, range);
    }

    private void Start()
    {
        var targetSphere = transform.GetChild(2);
        tgtAgent = targetSphere.GetComponent<TargetingAgent>();
    }
    void Shoot()
    {
        GameObject bulletObj = (GameObject)Instantiate(bulletPrefab, bulletSpawnTransform.position, gameObject.transform.rotation); // Instantiaite
        bulletObj.transform.rotation = tHead.transform.rotation; // orient bullet to the turret firing point's rotation
        Bullet bullet_CS = bulletObj.GetComponent<Bullet>();
        bullet_CS.speed *= shotVelocityMult; // after creating the bullet, multiply the speed immediately
        bullet_CS.dmg += dmg_Mod;
        if (bullet_CS != null)
        {
            bullet_CS.SetTargetTransform(this.tgt_Transform);
        }
    }
    private void OnEnable()
    {
        if (gameObject.transform.parent != null)
        {
            m_Team = GetComponentInParent<TurretBuilder>().m_Team; // On turret enable, check if its a child, and if there's a TurretBuilder in the parent.

        }
        else
        {
            Debug.Log(gameObject.name + " is ORPHANED!"); // if no TurretBuilder in the parent, then the turret is not connected to a base.
        }
        //Debug.Log("m_Team is " + m_Team);
        Light glow = transform.GetChild(0).GetComponent<Light>();

        switch (m_Team)
        {
            case Team.Neutral:
                e_Team = Team.Neutral;
                break;
            case Team.Blue:
                e_Team = Team.Red;
                glow.color = Color.blue;
                break;
            case Team.Red:
                e_Team = Team.Blue;
                glow.color = Color.red;
                break;
        }

        InvokeRepeating("UpdateTarget", 0f, 0.42f); // This is called when the turret is "Built"
    }
    private void OnDisable()
    {
        // On turret disable, set back to neutral and reset targeting information
        tgt_Transform = null;
    }


    void UpdateTarget()
    {
        tgt_Transform = tgtAgent.RequestClosestTarget();
        if (tgt_Transform == null)
        {
            targetName = "Out of Range";
        } else
        {
            targetName = tgt_Transform.name;
        }
    }


    /*
     * 
    void UpdateTarget()
    {

        
        if (m_Team == Team.Neutral)
            enemies = GameObject.FindGameObjectsWithTag("NPC"); // Find all NPCs
        else
            enemies = teamManager.FindTeamUnits(e_Team);
  
        float shortestDistance = float.MaxValue; // The shortest distance starts at infinity


        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
                return;
            // Out of all enemies, find the one with the shortest distance to the turret base
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        } // Found nearest enemy

        if (nearestEnemy != null && shortestDistance <= range)
        {
            // && !Physics.Raycast(turretHead.transform.position, nearestEnemy.transform.position, 100)
            //if nearest enemy is not null, and it's within our range, and the raycast does not hit a collider (false)
            targetTransform = nearestEnemy.transform; // store the transform of our nearest target.
            targetName = targetTransform.name;
            enemyInRange = true;
        }
        else if (nearestEnemy != null && shortestDistance > range)
        {
            targetName = "Out of Range";
            enemyInRange = false;
            targetTransform = null;
        }

    }
     */


    void Update()
    {
        // angles to rotate 
        if (tgt_Transform == null)
            return; // will return null if there are no enemy units within the targeting area
        
        Vector3 dir = tgt_Transform.position - (tHead.transform.position); // get the difference between points
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up); // have unity calculate quaternion based on this difference
        tHead.transform.rotation = Quaternion.Lerp(tHead.transform.rotation, rotation, 0.9f); // interpolate from current rotation to the one facing the target


            if (tm <= 0f)
            {
                Shoot();
                tm = fireRate;
            }

            tm -= Time.deltaTime;

    }
}
