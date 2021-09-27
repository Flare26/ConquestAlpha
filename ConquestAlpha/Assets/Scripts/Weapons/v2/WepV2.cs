using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WepV2 : MonoBehaviour
{
    // Start is called before the first frame update

    public WepSpec stats; // Scriptable Object!
    public GameObject projectile;
    
    private float reloadTime;
    private int clipSize;
    private int clipCurrent;
    private int dps; //damage per shot
    private float curr_fireRate;
    public bool reloading = false;
    public Transform[] firePoints;
    private bool holdToShoot;
    private bool readyToShoot;
    private bool shooting0;
    private bool shooting1;
    private int bulletsShot;
    private int bulletsPerTap;
    private int activeFP;
    private Text ammo;
    private Text rld;
    public float spinupTime;
    public float maxFireRate;
    private float curr_spinup;
    private void Start()
    {
        Debug.Log("Loaded weapon prefab: " + stats.wepName);
        ammo = GameObject.Find("AmmoTXT").GetComponent<Text>();
        rld = GameObject.Find("RldTXT").GetComponent<Text>();
        
    }
    private void OnEnable()
    {
        reloadTime = stats.reloadTime;
        clipSize = stats.ammo_clipSize;
        clipCurrent = clipSize;
        dps = stats.dps;
        curr_fireRate = stats.timeBetweenShot;
        holdToShoot = stats.allowHoldToShoot;
        name = stats.wepName;
        readyToShoot = true;
    }

    public bool Reload()
    {
        if (clipCurrent < clipSize && reloading == false)
        {
            reloading = true;
            Invoke("ReloadFinish", reloadTime);
            return reloading;
        }
        return reloading;
    }

    void ReloadFinish()
    {
        clipCurrent = clipSize;
        reloading = false;
        curr_spinup = spinupTime;
    }

    void WepControls()
    {
        // MOVE this over to player manager and make it work
        // Weapon controls is called within update
        if (holdToShoot)
        {
            shooting0 = Input.GetKey(KeyCode.Mouse0);
            shooting1 = Input.GetKey(KeyCode.Mouse1);
        }

          
        else
        {
            shooting0 = Input.GetKeyDown(KeyCode.Mouse0);
            shooting1 = Input.GetKeyDown(KeyCode.Mouse1);
        }
        //Debug.Log(shooting0);
        if (Input.GetKeyDown(KeyCode.R) && clipCurrent < clipSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting0 && !reloading && clipCurrent > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
            Debug.Log("Pew!");
        }
        // Auto Reload
        if (shooting0 && clipCurrent == 0 && !reloading)
            Reload();

    }

    private void Shoot()
    {

        readyToShoot = false;
        // Rotate through multiple firepoints
        if ( activeFP < firePoints.Length - 1)
        {
            activeFP++;
        } else
        {
            activeFP = 0;
        }

        // Instantiate bullet at the active firepoint
        Instantiate(projectile, firePoints[activeFP].position, transform.rotation);

        clipCurrent--;
        bulletsShot--;
        Invoke("ResetShot", curr_fireRate);
        if (bulletsShot > 0 && clipCurrent > 0)
            Invoke("Shoot", curr_fireRate);
    }

    public void ResetShot()
    {
        readyToShoot = true;
    }
    void Update()
    {
        ammo.text = "[" +clipCurrent+"/"+ clipSize+"]";
        if (reloading)
        {
            rld.enabled = true;
            ammo.enabled = false;
        } else
        {
            rld.enabled = false;
            ammo.enabled = true;
        }
            


            WepControls();
    }
}
