using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WepV2 : MonoBehaviour
{
    // Start is called before the first frame update

    public WepStats stats; // Scriptable Object!
    public float reloadTime;
    public int ammo_max;
    public int ammo_current;
    public int dps; //damage per shot
    public float timeBetweenShot;
    public bool reloading = false;

    private void Start()
    {
        reloadTime = stats.reloadTime;
        ammo_max = stats.ammo_max;
        ammo_current = ammo_max;
        dps = stats.dps;
        timeBetweenShot = stats.timeBetweenShot;
    }
    public bool Reload()
    {
        if (ammo_current < ammo_max && reloading == false)
        {
            reloading = true;
            Invoke("ReloadFinish", reloadTime);
            return reloading;
        }
        return reloading;
    }

    private void ReloadFinish()
    {
        ammo_current = ammo_max;
        reloading = false;
    }
}
