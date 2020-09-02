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
    [SerializeField] List<GameObject> inRange;
    TeamManager tm;
    Mode mode;

    public void Start()
    {
        //GameEvents.current.onDeathRemoveAggro += 
    }

    private void RemoveAggro()
    {

    }
    private void OnEnable()
    {
        inRange = new List<GameObject>();
        tm = GetComponentInParent<TeamManager>();
        // Assess whether or not this agent will acquire targets for a turret, or NPC
        //Initialize based on what type of unit the target agent is on
        if (transform.parent.tag == "Turret")
        {
            Debug.Log("Initializing TargetingAgent | Turret");
            var tm = GetComponentInParent<TeamManager>();
            var ai = GetComponentInParent<TurretAI>();
            GetComponent<SphereCollider>().radius = ai.range; // set the range collider to proper size based on the turret range stat
        }
        else if (transform.parent.tag == "NPC")
        {
            Debug.Log("Initializing TargetingAgent | Turret");
            var ai = GetComponentInParent<NPCAI>();
            GetComponent<SphereCollider>().radius = ai.range;

        }
        else
        {
            Debug.Log("Targeting Agent has incorrect/null parent!");
        }
        
        
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (mode)
        {
            case Mode.Turret:
                inRange.Add(other.gameObject);
                TurretRoutine(other);
                break;
            case Mode.NPC:
                inRange.Add(other.gameObject);
                NPCRoutine(other);
                break;
        }
    }


    // TurretRoutine and NPCRoutine both populate the hostiles list however they filter what gets added to it differently.
    private void TurretRoutine(Collider other)
    {
        try
        {
            TeamManager tm;
            if (other.gameObject.TryGetComponent<TeamManager>(out tm))
                if (!tm.m_Team.Equals(tm.m_Team))
                    hostiles.Add(other.gameObject);
        }
        catch(Exception e)
        {
            Debug.Log("Exeption in turret routine");
        }
    }

    private void NPCRoutine(Collider other)
    {
        // NPC Can target players, Turrets, other NPC
        Team ut = Team.Neutral;

        if (other.tag == "NPC" || other.tag == "TurretHead" || other.tag == "Player")
        {

            switch (other.tag)
            {
                case "NPC":
                    ut = other.GetComponent<TeamManager>().m_Team; // check team of new potential target

                    break;
                case "TurretHead":
                    ut = other.GetComponentInParent<TeamManager>().m_Team; // check team of new potential target. TurretHead should always have a parent so we can assume it has a TurretAi
                    break;
                case "Player":
                    Debug.Log("Tried to target " + other.name) ;
                    break;
            } // The team of the enemy has now been determined

            if (!ut.Equals(tm.m_Team))
                hostiles.Add(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        hostiles.TrimExcess();
        // When units die within the targeting range, a data ghost is left as null in the enemy units array. To fix this, regularly remove nulls
        for (int i = 0; i < hostiles.Count; i++)
        {
            // Verify that we have not targeted our own team somehow!
            TeamManager htm; // hostile team manager
            var hostile = hostiles[i];
            if (hostile.gameObject.TryGetComponent<TeamManager>(out htm))
            {
                Debug.Log("HTM" + htm);
                Debug.Log("TM"+tm);
                if (htm.m_Team == tm.m_Team)
                    hostiles.Remove(hostiles[i]);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (hostiles.Contains(other.gameObject))
        {
            hostiles.Remove(other.gameObject);
        }
    }

    public Transform RequestClosestTarget(TeamManager m_tm)
    {
        if (hostiles.Count == 0)
            return null;
        TeamManager etm; // enemy team manager

        for (int i = 0; i < hostiles.Count; i++)
        {
            if (hostiles[i].gameObject.TryGetComponent<TeamManager>(out etm))
                if (m_tm.m_Team.Equals(tm.m_Team))
                    hostiles.RemoveAt(i); // remove friendlies 
        }

            float dist_low = float.MaxValue;
            Transform close_targ = null;
            for (int i = 0; i < hostiles.Count; i++)
            {
                
                Transform t = hostiles[i].transform;
                if (t.Equals(null))
                    return null;
                float dist_targ = Vector3.Distance(transform.position, t.transform.position);
                if (dist_targ < dist_low )
                {
                    dist_low = dist_targ;
                    close_targ = t;
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
