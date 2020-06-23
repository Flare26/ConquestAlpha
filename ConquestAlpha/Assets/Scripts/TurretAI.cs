using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [SerializeField]
    private string team;
    public float turnSpeed;
    public float range = 15f;
    public float fireRate = 1f;
    private float fireCooldown = 0f;
    public string enemyTag = null; // Enemy tag can change relative to the team this turret belongs to. For now, just using NPCTank for testing.

    public GameObject bulletPrefab;
    public GameObject enemy;
    public GameObject turretHead;
    public Transform bulletSpawnTransform; // use an empty game object as reference for fire point so bullet does not spawn within turret

    Transform targetTransform;
    // Start is called before the first frame update
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(turretHead.transform.position, range);
    }
    void Start()
    {
        Transform [] childs = gameObject.GetComponentsInChildren<Transform>();
        foreach ( Transform child in childs )
        {
            if (child.tag == "TurretHead")
            {
                turretHead = child.gameObject;
            }
        }
        
    }
    private void OnEnable()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.25f); // This is called when the turret is "Built"
        if (gameObject.transform.parent != null)
            team = GetComponentInParent<TurretBuilder>().team; // On turret enable, check if its a child, and if there's a TurretBuilder in the parent.
        else
            Debug.Log(gameObject.name + " IS AN ORPHAN!"); // if no TurretBuilder in the parent, then the turret is not connected to a base.

        if (team == "Blue")
        {
            enemyTag = "NPCRed";
        }
        else if (team == "Red")
        {
            enemyTag = "NPCBlue";
        }

    }
    private void OnDisable()
    {
        team = "neutral"; // On turret disable, set back to neutral and reset targeting information
        enemyTag = null;
    }
    void Shoot()
    {
        GameObject bulletObj = (GameObject) Instantiate(bulletPrefab, bulletSpawnTransform.position, gameObject.transform.rotation); // Instantiaite
        bulletObj.transform.rotation = turretHead.transform.rotation;
        Bullet bulletCS = bulletObj.GetComponent<Bullet>();
        if (bulletCS != null)
        {
            bulletCS.SetTargetTransform(this.targetTransform);
        }
        //Debug.Log("Shoot!");
    }

    void UpdateTarget()
    {
        // Called by invokerepeating in onenable method
        // If the enemy tag has not been set, the turret has not been built & assigned a team yet. Return.
        if (enemyTag == null)
            return;
 
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = float.MaxValue; // The shortest distance starts at infinity
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            // Out of all enemies, find the one with the shortest distance to the turret base
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        } // end foreach and we now have the true nearest enemy to this turret stored in nearestEnemy. Now check to see if the enemy fufills our range requirement.

        if ( nearestEnemy != null && shortestDistance <= range && !Physics.Raycast(turretHead.transform.position, nearestEnemy.transform.position, range * 2))
        {
            //
            //if nearest enemy is not null, and it's within our range, and the raycast does not hit a collider (false)
            targetTransform = nearestEnemy.transform; // store the transform of our nearest target.
        } else
        {
            // If the nearestEnemy is behind a collider or outside of our range, set the transform to null. See first 3 lines of Update()
            targetTransform = null;
        }
    }

    void Update()
    {
        if (targetTransform == null)
            return;

        // angles to rotate 

        Vector3 dir = targetTransform.position - turretHead.transform.position; // get the difference between points
        Quaternion rotation = Quaternion.LookRotation(dir); // have unity calculate quaternion based on this difference
        turretHead.transform.rotation = Quaternion.Lerp(turretHead.transform.rotation, rotation, turnSpeed * Time.deltaTime); // interpolate from current rotation to the one facing the target

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1 / fireRate;
        }

        fireCooldown -= Time.deltaTime;
    } 
}
