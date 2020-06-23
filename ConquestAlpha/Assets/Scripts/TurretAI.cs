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
    public Transform target;
    public Transform firePoint; // use an empty game object as reference for fire point so bullet does not spawn within turret
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
            team = GetComponentInParent<TurretConstructor>().team; // On turret enable, set appropriate team and targeting info
        else
            Debug.Log(gameObject.name + " IS AN ORPHAN!");

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
        GameObject bulletObj = (GameObject) Instantiate(bulletPrefab, firePoint.position, gameObject.transform.rotation); // Instantiaite
        //bulletObj.GetComponent<NewBullet>().bulletSpawn = firePoint; // Set the bullet spawn transform to store orientation data of the turret
        bulletObj.GetComponent<Transform>().rotation = gameObject.transform.rotation;
        Bullet bulletCS = bulletObj.GetComponent<Bullet>();
        if (bulletCS != null)
        {
            bulletCS.SetTarget(target);
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
            target = nearestEnemy.transform;
        } else
        {
            target = null;
        }
    }

    void Update()
    {
        // No clue how this shit works
        if (target == null)
            return;

        //Target Lock On
            Vector3 dir = target.position - transform.position; // get the vector direction from the current position
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotationValues = Quaternion.Lerp(turretHead.transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles; // head transform current is the start, the new rotation is the finish
            turretHead.transform.rotation = Quaternion.Euler(0f, rotationValues.y, 0f); // only rotate around Y axis because otherwise the whole thing will rotate
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1 / fireRate;
        }

        fireCooldown -= Time.deltaTime;
    } 
}
