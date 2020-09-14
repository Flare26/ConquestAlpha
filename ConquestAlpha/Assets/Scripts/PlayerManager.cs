﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Nathan Frazier
public class PlayerManager : MonoBehaviour, IRespawnable
{
    [SerializeField] Slider hpSlider;
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            primary.GetComponent<WeaponCore>().Fire(primaryBulletSpawn.position, GetComponent<Transform>().rotation);
            secondary.GetComponent<WeaponCore>().Fire(secondaryBulletSpawn.position, GetComponent<Transform>().rotation);
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
