using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Weapon Stats")]
public class WepStats : ScriptableObject
{
    public string wepName;
    public float reloadTime;
    public float timeBetweenShot;
    public int ammo_clipSize;
    public int ammo_clipCurrent;
    
    public int dps; //damage per shot
    public bool reloading = false;
    public bool allowHoldToShoot = false;
}
