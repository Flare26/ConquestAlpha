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
        // This is called when the turret is "Built"

        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        if (gameObject.transform.parent != null)
            team = GetComponentInParent<TurretConstructor>().team; // On turret enable, check if its a child, and if there's a TurretConstructor in the parent.
        else
            Debug.Log(gameObject.name + " IS AN ORPHAN!"); // if no TurretConstructor in the parent, then the turret is not connected to a base.

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
        CancelInvoke(); // Cancels the invoke repeating called when the turret is enabled
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

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(turretHead.transform.position, range);
    }
    void UpdateTarget()
    {
        // If the enemy tag has not been set, the turret has not been built & assigned a team yet. Return.
        if (enemyTag == null)
            return;
 
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = float.MaxValue;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        } // end foreach

        if (nearestEnemy != null && shortestDistance <= range)
        {
            targetTransform = nearestEnemy.transform;
        } else
        {
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
