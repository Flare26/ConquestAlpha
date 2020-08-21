using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int hull;
    public int shield;
    int shield_rate;
    public GameObject weapon1Obj;
    public GameObject weapon2Obj;
    public Transform primaryBulletSpawn;
    public Transform secondaryBulletSpawn;
    GameObject primary;
    GameObject secondary;
    ClassType type;
    public Team m_Team;
    public UnitClass classScript;

    public Transform primaryMount;
    public Transform secondaryMount;

    // Start is called before the first frame update
    void OnEnable()
    {
        //give the proper stats based on the class script this player is using

        primary = Instantiate<GameObject>(weapon1Obj, primaryMount);
        primary.GetComponent<WeaponCore>().bulletSpawn = primaryBulletSpawn;
        secondary = Instantiate<GameObject>(weapon2Obj, secondaryMount);
        secondary.GetComponent<WeaponCore>().bulletSpawn = secondaryBulletSpawn;
    }

    private void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            primary.GetComponent<WeaponCore>().Fire(primaryBulletSpawn.position, GetComponent<Transform>().rotation);
        }
    }

}
