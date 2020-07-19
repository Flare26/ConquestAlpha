using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTurretAI : MonoBehaviour
{
    [SerializeField] Team m_Team;
    [SerializeField] TeamManager teamManager;
    [SerializeField] Team enemyTeam;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnTransform; // use an empty game object as reference for fire point so bullet does not spawn within turret
    [SerializeField] GameObject tHead;
    [SerializeField] float fireRate; // shots per sec

    static GameObject[] enemies;
    private float fireCooldown = 0f;
    public float turnSpeed; // clamped 0-1
    public float range;
    bool enemyInRange = false;
    GameObject nearestEnemy = null;
    Transform targetTransform;
    public int shotVelocityMult;
    public string targetName = "No Target";
    // Start is called before the first frame update
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(tHead.transform.position, range);
    }
    void Shoot()
    {
        GameObject bulletObj = (GameObject)Instantiate(bulletPrefab, bulletSpawnTransform.position, gameObject.transform.rotation); // Instantiaite
        bulletObj.transform.rotation = tHead.transform.rotation;
        Bullet bullet_CS = bulletObj.GetComponent<Bullet>();
        bullet_CS.speed *= shotVelocityMult;
        if (bullet_CS != null)
        {
            bullet_CS.SetTargetTransform(this.targetTransform);
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
            Debug.Log(gameObject.name + " IS ORPHAN!"); // if no TurretBuilder in the parent, then the turret is not connected to a base.
        }

        if (m_Team == Team.Blue)
        {
            enemyTeam = Team.Red;
        }
        else if (m_Team == Team.Red)
        {
            enemyTeam = Team.Blue;
        }
        InvokeRepeating("UpdateTarget", 0f, 0.42f); // This is called when the turret is "Built"
    }
    private void OnDisable()
    {
        // On turret disable, set back to neutral and reset targeting information
        targetTransform = null;
    }
    void UpdateTarget()
    {
        if (m_Team == Team.Neutral)
            enemies = GameObject.FindGameObjectsWithTag("NPC"); // Find all NPCs
        else
            enemies = teamManager.FindTeamUnits(enemyTeam);
  
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

    void Update()
    {
        // angles to rotate 
        if (targetTransform == null)
            return;
        // aim - ahead test
        
        Vector3 dir = targetTransform.position - (tHead.transform.position); // get the difference between points
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up); // have unity calculate quaternion based on this difference
        tHead.transform.rotation = Quaternion.Lerp(tHead.transform.rotation, rotation, 0.9f); // interpolate from current rotation to the one facing the target

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate;
        }

        fireCooldown -= Time.deltaTime;

    }
}
