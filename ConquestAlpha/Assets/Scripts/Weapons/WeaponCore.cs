using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCore : MonoBehaviour
{
    [Header("Customize The Fields!")]
    public string m_WepName;
    public GameObject projectile;
    public Transform bulletSpawn;
    public float shotVelocityMult = 1;

    private void OnEnable()
    {
        
    }
    public void Fire(Vector3 firePoint, Quaternion rotation)
    {
        GameObject bulletObj = Instantiate(projectile, firePoint, rotation); // Instantiaite
        bulletObj.GetComponent<Bullet>().speed *= shotVelocityMult; // after creating the bullet, multiply the speed immediately
    }
}
