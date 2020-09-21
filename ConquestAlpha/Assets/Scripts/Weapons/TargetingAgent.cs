using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

// CURRENTLY CRASHES BECUASE ITS COUNTING BULLETS AS HOSTILES FOR SOME REASON
public class TargetingAgent : MonoBehaviour
{
    enum Mode
    {
        NPC,
        Turret
    }
    // This script at it's core should search for a suitable target in range for the turret that it is a parent of. It should present the turret with a target every time it requests one.
    // Made to be used with a targetArea collision trigger sphere or object 
    [SerializeField] public List<GameObject> hostiles;
    [SerializeField] public List<TargetingAgent> inRange;
    [SerializeField] SphereCollider targetingArea;
    TeamManager tm;
     public Team myTeam = Team.Neutral;
    Mode mode;

    private void OnEnable()
    {
        inRange = new List<TargetingAgent>();
        tm = GetComponentInParent<TeamManager>();
        myTeam = tm.m_Team;

        // Assess whether or not this agent will acquire targets for a turret, or NPC
        //Initialize based on what type of unit the target agent is on
        if (gameObject.tag == "Turret")
        {
            //Debug.Log("Initializing TargetingAgent | Turret");
            var tm = GetComponentInParent<TeamManager>();
            var ai = GetComponentInParent<TurretAI>();
            targetingArea.radius = ai.range; // set the range collider to proper size based on the turret range stat
        }
        else if (gameObject.tag == "NPC")
        {
            //Debug.Log("Initializing TargetingAgent | NPC");
            var ai = GetComponentInParent<NPCAI>();
            targetingArea.radius = ai.range;

        }
        else
        {
            Debug.Log("Targeting Agent has incorrect/null parent!");
        }
        
        
    }

    // TurretRoutine and NPCRoutine both populate the hostiles list however they filter what gets added to it differently.
    private void TurretRoutine()
    {
        foreach (TargetingAgent g in inRange)
        {
            TeamManager utm; // unit team manager
            if (g.TryGetComponent<TeamManager>(out utm))
            {
                // For each team manager, compare the teams and if they are different then add the gameobject of the utm to hostiles.
                if (!utm.m_Team.Equals(GetComponent<TeamManager>().m_Team))
                    if (utm.gameObject.tag.Equals("NPC") && !hostiles.Contains(g.gameObject))
                    hostiles.Add(utm.gameObject);
            }
        }
    }

    private void NPCRoutine()
    {
        foreach (TargetingAgent g in inRange)
        {
            // if something dies in the middle of this routine check for that.
            if (g == null)
                return;

            TeamManager utm;
            if (g.tag.Equals("NPC") || g.tag.Equals("TurretHead"))
            {
                if ( g.TryGetComponent<TeamManager>(out utm) )
                {
                    if (!utm.m_Team.Equals(myTeam) && !hostiles.Contains(g.gameObject))
                        hostiles.Add(g.gameObject);
                }
            }
        }
    }

    public Transform RequestClosestTarget()
    {
        // In-range includes both turrets and NPCS. Set the hostiles appropriately 

        switch (mode)
        {
            case Mode.Turret:
                TurretRoutine();
                break;
            case Mode.NPC:
                NPCRoutine();
                break;
        }

        if (hostiles.Count == 0)
            return null;
        TeamManager etm; // enemy team manager

            float dist_low = float.MaxValue;
            Transform close_targ = null;
            for (int i = 0; i < hostiles.Count; i++)
            {
                if (!hostiles[i].Equals(null))
                {
                    Transform t = hostiles[i].transform;
                    float dist_targ = Vector3.Distance(transform.position, t.transform.position);
                    if (dist_targ < dist_low )
                    {
                        dist_low = dist_targ;
                        close_targ = t;
                    }

                }
            }

        return close_targ;
    }

    public void RequestWeakestTarget()
    {
        //not implemented
    }

    public void RequestStrongestTarget()
    {
        //not implemented
    }

}
