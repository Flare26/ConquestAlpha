using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tank
{
    // Start is called before the first frame update
    char team; // 'r' red 'b' blue
    int hull; // HP
    int sMax; // shieldMax
    int sCur; // shieldCurrent
    int sR; // shield recharge rate
    GameObject deathParticles;

    UnitType unitClass;

    public enum UnitType
    {
        Light,
        Heavy,
        Sniper
    }

    public Tank(UnitType type, char team)
    {
        this.team = team;
        unitClass = type;
    }

    // All tanks should have a public take damage
    public void TakeDamage(int dmg)
    {
        if (dmg == sCur)
            sCur = 0;

        if ( dmg > sCur )
        {
            //if incoming damage is greater than current shields, find difference and sub from hull
            hull -= dmg - sCur;
        }
    }
}
