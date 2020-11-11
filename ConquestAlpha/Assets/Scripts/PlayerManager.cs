using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Nathan Frazier
public class PlayerManager : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    public int playerNumber = 0;
    public int currentHull;
    public float currentShield;
    public int maxHull;
    public int maxShield;
    public float shieldRate;
    public float shieldDelay;
    public int pps; // projectile per second
    public GameObject weapon1Obj;
    public GameObject weapon2Obj;
    
    public Transform primaryBulletSpawn;
    public Transform secondaryBulletSpawn;
    GameObject primaryInst;
    GameObject secondaryInst;
    
    public List<GameObject> targetedBy = new List<GameObject>();
    public Transform primaryMount;
    public Transform secondaryMount;
    public static Quaternion defaultAim;
    PlayerParticles particleManager;
    public float sinceLastDMG = 0f;
    bool hasShield = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        hasShield = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentHull = maxHull;
        currentShield = maxShield;
        hpSlider.maxValue = maxHull;
        hpSlider.value = currentHull;

        //give the proper stats based on the class script this player is using
        //Change the glow color of your hitbox
        var glowLight = transform.Find("TeamLight");
        Light glowColor = glowLight.GetComponent<Light>();
        particleManager = GetComponent<PlayerParticles>();
        switch (GetComponent<TeamManager>().m_Team)
        {
            //assign by team
            case Team.Red:
                glowColor.color = Color.red;
                Debug.Log("Set Glow To Red!");
                break;

            case Team.Blue:
                glowColor.color = Color.blue;
                Debug.Log("Set Glow To Blue!");
                break;
            default:
                glowColor.color = Color.white;
                break;
        }

        primaryInst = Instantiate<GameObject>(weapon1Obj, primaryMount);
        secondaryInst = Instantiate<GameObject>(weapon2Obj, secondaryMount);
        primaryBulletSpawn = primaryInst.transform.Find("FP"); // find firepoints inside the instance
        secondaryBulletSpawn = secondaryInst.transform.Find("FP");
        //defaultAim = primaryBulletSpawn.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        String name = other.gameObject.name;


        if (name.Equals("TargetingArea"))
        {
            var enemyTargList = other.gameObject.GetComponentInParent<NPCTargetingAgent>().inRange;
            var enemyHostileList = other.gameObject.GetComponentInParent<NPCTargetingAgent>().hostiles;
            var myTeam = GetComponent<TeamManager>().m_Team;
            var eTeam = other.gameObject.GetComponentInParent<TeamManager>().m_Team;
            if (enemyTargList.Contains(gameObject))
                return;

            Debug.Log("Unit " + gameObject.name + " has moved into the targeting area of " + other.gameObject.name);
            enemyTargList.Add(gameObject);

            if (!myTeam.Equals(eTeam))
                enemyHostileList.Add(gameObject);
        }
  
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag.Equals("Bullet"))
        {
            Debug.Log("Player was hit!");
            particleManager.BulletHitFX();
            TakeDamage(c.gameObject.GetComponent<Bullet>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        String name = other.gameObject.name;
        NPCTargetingAgent tmp;

        if (name.Equals("TargetingArea"))
        {
            tmp = other.gameObject.GetComponentInParent<NPCTargetingAgent>();

            Debug.Log("Unit " + gameObject.name + " has moved outside of targeting area of " + tmp.gameObject.name);
            tmp.inRange.Remove(gameObject);
            tmp.hostiles.Remove(gameObject);
        }
    }

    public void TakeDamage(Bullet b)
    {
        var indmg = b.m_dmg;
        sinceLastDMG = 0;

        if (currentShield < indmg && hasShield)
        {
            particleManager.PopShield();
            // incoming dmg greater than shield, sub shield from dmg and apply to HP
            int bleed = Mathf.RoundToInt(indmg - currentShield);
            currentHull -= bleed;
            currentShield = 0;
            // Start the shield regen count at 0
            //Debug.Log("Dmg made it past shield!");
        }
        else if (hasShield && currentShield - indmg != 0)
        {
            //Debug.Log("Shield absorbs dmg");
            // incoming dmg is either same as shield or less so sub from shield
            currentShield -= indmg;
        }
        else if (currentShield - indmg == 0)
        {
            Debug.Log("Perfect Pop");
            currentShield = 0;
            hasShield = false;
            particleManager.PopShield();
        }
        else if (!hasShield)
        {
            currentHull -= indmg;
            //Debug.Log("Took direct hit while shield DOWN! ");
        }

        Destroy(b.gameObject);
    }
    public void SetSpawn(Vector3 spawnPoint)
    {
        throw new System.NotImplementedException();
    }

    public void ReturnToBuildQueue(GameObject parent)
    {
        throw new System.NotImplementedException();
    }

    void CheckShieldCharge()
    {
        sinceLastDMG += Time.deltaTime;
        if (sinceLastDMG > shieldDelay && currentShield < maxShield)
        {
            currentShield += Time.deltaTime * shieldRate;
            hasShield = true;
        }
        else if (currentShield > maxShield)
        {
            currentShield = maxShield;
            hasShield = true;
        }
    }
    private void Update()
    {
        CheckShieldCharge();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            primaryInst.GetComponent<WeaponCore>().CheckReload();
            secondaryInst.GetComponent<WeaponCore>().CheckReload();
            primaryInst.GetComponent<WeaponCore>().Fire();
            secondaryInst.GetComponent<WeaponCore>().Fire();
        }
    }
}
