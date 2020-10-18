using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using UnityEngine.AI;

public class NPC : GameUnit, IKillable
{
    // Nathan Frazier
    // these are going to be created and built with the team of the base

    HoverController_AI driver;
    ParticlePlayer fxplayer;
    TeamManager tm;
    TargetingAgent ta;
    private void Awake()
    {
        // Triggered when script is loaded into runtime
        if (!TryGetComponent<HoverController_AI>(out driver))
            Debug.LogError("NPC Hover tank has no hover controller script! >" + gameObject.name );

        if (!TryGetComponent<ParticlePlayer>(out fxplayer))
            Debug.LogError("NPC does not have a particle player script! >" + gameObject.name);

        if (!TryGetComponent<TeamManager>(out tm))
            Debug.LogError("NPC does not have a Team Manager! >" + gameObject.name);
        if (!TryGetComponent<TargetingAgent>(out ta))
            Debug.LogError("NPC does not have a TargetingAgent! >" + gameObject.name);
    }

    void OnEnable()
    {
        // This is triggered every time NPC respawns. Do this instead of destroying
        h_current = h_max;
        sh_current = sh_max;
        hasShield = true;

        primaryInstance = Instantiate<GameObject>(primaryWep, mount_Primary);
        secondaryInstance = Instantiate<GameObject>(secondaryWep, mount_Secondary);
        InvokeRepeating("RefreshCurrentTarget", 0.5f, 0.5f);
    }
    void RefreshCurrentTarget()
    {
        curr_targ = ta.RequestClosestTarget();
        if(curr_targ != null)
        {
            var lookvector = curr_targ.position;
            lookvector.y = transform.position.y; // this is to prevent the whole vehicle from turning itself up / down for now
            transform.LookAt(lookvector);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        String name = other.gameObject.name;


        if (name.Equals("TargetingArea"))
        {
            var enemyTargList = other.gameObject.GetComponentInParent<TargetingAgent>().inRange;
            var enemyHostileList = other.gameObject.GetComponentInParent<TargetingAgent>().hostiles;
            var myTeam = GetComponent<TeamManager>().m_Team;
            var eTeam = other.gameObject.GetComponentInParent<TeamManager>().m_Team;
            if (enemyTargList.Contains(gameObject))
                return;

            Debug.Log("Unit " + gameObject.name + " has moved into the targeting area of " + other.gameObject.name);
            enemyTargList.Add(gameObject);

            if (!myTeam.Equals(eTeam))
                enemyHostileList.Add(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            TakeDamage(collision.gameObject.GetComponent<Bullet>());
    }

    private void OnCollisionExit(Collision collision)
    {
 
    }

    public void TakeDamage(Bullet b)
    {
        var indmg = b.m_dmg;
        sinceLastDMG = 0;
        
        if (sh_current < indmg && hasShield)
        {
            fxplayer.PopShield();
            // incoming dmg greater than shield, sub shield from dmg and apply to HP
            int bleed = Mathf.RoundToInt( indmg - sh_current );
            h_current -= bleed;
            sh_current = 0;
            // Start the shield regen count at 0
            //Debug.Log("Dmg made it past shield!");
        } else if (hasShield && sh_current - indmg != 0)
        {
            //Debug.Log("Shield absorbs dmg");
            // incoming dmg is either same as shield or less so sub from shield
            sh_current -= indmg;
        } else if (sh_current - indmg == 0)
        {
            Debug.Log("Perfect Pop");
            sh_current = 0;
            hasShield = false;
            fxplayer.PopShield();
        } else if (!hasShield)
        {
            h_current -= indmg;
            //Debug.Log("Took direct hit while shield DOWN! ");
        }
    }

    private void FixedUpdate()
    {
        //first check for death
        if (h_current < 1)
        {
            DeathRoutine();
        }

        if (curr_targ != null)
        {
            primaryInstance.GetComponent<WeaponCore>().CheckReload();
            secondaryInstance.GetComponent<WeaponCore>().CheckReload();
            primaryInstance.GetComponent<WeaponCore>().Fire();
            secondaryInstance.GetComponent<WeaponCore>().Fire();
        }
    }
    public void DeathRoutine()
    {
        fxplayer.DeathFX();
        gameObject.SetActive(false);
    }
}
