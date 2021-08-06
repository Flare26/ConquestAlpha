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
    public float reloadTime;
    public int clipSize;
    public int clipCurrent;
    public int dps; //damage per shot
    public float timeBetweenShot;
    public bool reloading = false;
    public Transform[] firePoints;
    public bool holdToShoot;
    public bool readyToShoot;
    public bool shooting;
    private int bulletsShot;
    private int bulletsPerTap;
    private int activeFP;
    private Text ammo;
    private Text rld;
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
        timeBetweenShot = stats.timeBetweenShot;
        holdToShoot = stats.allowHoldToShoot;
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
    }

    void WepControls()
    {
        // Weapon controls
        if (holdToShoot)
            shooting = Input.GetKey(KeyCode.Mouse0);
          
        else 
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        Debug.Log(shooting);
        if (Input.GetKeyDown(KeyCode.R) && clipCurrent < clipSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && clipCurrent > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
            Debug.Log("Pew!");
        }
        
    }

    private void Shoot()
    {

        
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
        Invoke("ResetShot", timeBetweenShot);
        if (bulletsShot > 0 && clipCurrent > 0)
            Invoke("Shoot", timeBetweenShot);
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
