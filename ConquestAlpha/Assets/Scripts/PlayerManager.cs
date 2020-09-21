using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Nathan Frazier
public class PlayerManager : MonoBehaviour, IRespawnable
{
    [SerializeField] Slider hpSlider;
    public int playerNumber = 0;
    public int currentHull;
    public int currentShield;
    public int maxHull;
    public int maxShield;
    int shield_rate;
    public int pps; // projectile per second
    public GameObject weapon1Obj;
    public GameObject weapon2Obj;
    public Transform primaryBulletSpawn;
    public Transform secondaryBulletSpawn;
    GameObject primary;
    GameObject secondary;
    public UnitClass classScript;
    public List<TargetingAgent> targetedBy = new List<TargetingAgent>();
    public Transform primaryMount;
    public Transform secondaryMount;

    // Start is called before the first frame update
    void OnEnable()
    {

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

        primary = Instantiate<GameObject>(weapon1Obj, primaryMount);
        primary.GetComponent<WeaponCore>().bulletSpawn = primaryBulletSpawn;
        secondary = Instantiate<GameObject>(weapon2Obj, secondaryMount);
        secondary.GetComponent<WeaponCore>().bulletSpawn = secondaryBulletSpawn;
    }

    private void OnTriggerEnter(Collider other)
    {
        String name = other.gameObject.name;
        TargetingAgent tmp;

        if (name.Equals("TargetingArea"))
        {
            tmp = other.gameObject.GetComponentInParent<TargetingAgent>();
            if (tmp.inRange.Contains(GetComponent<TargetingAgent>()))
                return;
            Debug.Log("Unit " + gameObject.name + " has moved into the targeting area of " + tmp.gameObject.name);
            tmp.inRange.Add(GetComponent<TargetingAgent>());
            if (!targetedBy.Contains(tmp))
                targetedBy.Add(tmp);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        String name = other.gameObject.name;
        TargetingAgent tmp;

        if (name.Equals("TargetingArea"))
        {
            tmp = other.gameObject.GetComponentInParent<TargetingAgent>();

            Debug.Log("Unit " + gameObject.name + " has moved outside of targeting area of " + tmp.gameObject.name);
            tmp.inRange.Remove(GetComponent<TargetingAgent>());
            targetedBy.Remove(tmp.GetComponent<TargetingAgent>());
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            primary.GetComponent<WeaponCore>().Fire(primaryBulletSpawn.position, primaryBulletSpawn.rotation);
            secondary.GetComponent<WeaponCore>().Fire(secondaryBulletSpawn.position, secondaryBulletSpawn.rotation);
        }
    }

    public void TakeDamage(int currentshields, int currenthull)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int dmg)
    {
        hpSlider.value = currentHull;
        throw new System.NotImplementedException();
    }

    public void SetSpawn(Vector3 spawnPoint)
    {
        throw new System.NotImplementedException();
    }

    public void ReturnToBuildQueue(GameObject parent)
    {
        throw new System.NotImplementedException();
    }
}
