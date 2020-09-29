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
    
    private void Awake()
    {
        // Triggered when script is loaded into runtime
        if (!TryGetComponent<HoverController_AI>(out driver))
            Debug.LogError("NPC Hover tank has no hover controller script! >" + gameObject.name );

        if (!TryGetComponent<ParticlePlayer>(out fxplayer))
            Debug.LogError("NPC does not have a particle player script! >" + gameObject.name);
    }

    void OnEnable()
    {
        // This is triggered every time NPC respawns. Do this instead of destroying
        h_current = h_max;
        sh_current = sh_max;
        hasShield = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            TakeDamage(collision.gameObject.GetComponent<Bullet>());
    }

    public void TakeDamage(Bullet b)
    {
        var indmg = b.m_dmg;

        
        if (sh_current < indmg && hasShield)
        {
            fxplayer.PopShield();
            // incoming dmg greater than shield, sub shield from dmg and apply to HP
            int bleed = (int) indmg - sh_current;
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
    }
    public void DeathRoutine()
    {
        fxplayer.DeathFX();
        gameObject.SetActive(false);
    }
}
