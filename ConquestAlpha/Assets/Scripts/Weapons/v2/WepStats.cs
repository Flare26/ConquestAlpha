using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Weapon Stats")]
public class WepStats : ScriptableObject
{
    public float reloadTime;
    public float timeBetweenShot;
    public int ammo_max;
    public int ammo_current;
    public int dps; //damage per shot
    public bool reloading = false;
}
