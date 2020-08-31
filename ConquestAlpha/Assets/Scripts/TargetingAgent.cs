using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;


public class TargetingAgent : MonoBehaviour
{
    enum Mode
    {
        NPC,
        Turret
    }
    // This script at it's core should search for a suitable target in range for the turret that it is a parent of. It should present the turret with a target every time it requests one.
    // Made to be used with a targetArea collision trigger sphere or object 
    [SerializeField] public List<Transform> hostiles;
    [SerializeField] public Team m_Team;
    Mode mode;
    Team e_Team; // If turret is neutral, this value remains NULL
    private void OnEnable()
    {
        // Assess whether or not this agent will acquire targets for a turret, or NPC
        if (transform.parent.tag == "Turret")
        {
            Debug.Log("Targeting mode: Turret");
            var ai = GetComponentInParent<TurretAI>();
            GetComponent<SphereCollider>().radius = ai.range; // set the range collider to proper size based on the turret range stat
            m_Team = ai.m_Team;

        }
        else if (transform.parent.tag == "NPC")
        {
            Debug.Log("Targeting mode: NPC");
            var ai = GetComponentInParent<NPCAI>();
            GetComponent<SphereCollider>().radius = ai.range;
            m_Team = ai.m_Team;
            
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
                TurretRoutine(other);
                break;
            case Mode.NPC:
                NPCRoutine(other);
                break;
        }
    }


    // TurretRoutine and NPCRoutine both populate the hostiles list however they filter what gets added to it differently.
    private void TurretRoutine(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Team pteam = other.transform.gameObject.GetComponent<TeamManager>().m_Team;
            Debug.Log("Pteam =" + pteam);
            if (pteam == m_Team)
                Debug.Log("Same Team!");
        }
        bool isNPC = false;
        Team ut = Team.Neutral;
        if (other.gameObject.CompareTag("NPC"))
        {
            ut = other.GetComponent<NPCAI>().m_Team;
            isNPC = true;
        }


        if (m_Team == Team.Neutral)
        {
            //Debug.Log("Target In Range");
            //if turret does not have a team (neutral) all NPC are potential targets
            if (isNPC)
            {
                hostiles.Add(other.transform);
            }
        }
        else
        {

            // if this turret has a team
            List<Transform> inRange = new List<Transform>();
            if (isNPC && ut != m_Team)
            {
                // and the intruder is on a different team
                hostiles.Add(other.transform);
            }
        }
    }

    private void NPCRoutine(Collider other)
    {
        // NPC Can target players, Turrets, other NPC
        bool isTgtable = false; // is this new found unit targetable by our NPC? If so, add to hostiles.
        Team ut = Team.Neutral;

        if (other.tag == "NPC" || other.tag == "TurretHead" || other.tag == "Player")
        {

            switch (other.tag)
            {
                case "NPC":
                    ut = other.GetComponent<NPCAI>().m_Team; // check team of new potential target
                    break;
                case "TurretHead":
                    ut = other.GetComponentInParent<TurretAI>().m_Team; // check team of new potential target. TurretHead should always have a parent so we can assume it has a TurretAi
                    break;
                case "Player":
                    Debug.Log("Tried to target " + other.name) ;
                    break;
            } // The team of the enemy has now been determined
            
            if (ut != m_Team)
            {
                isTgtable = true;
            }

        }


        if (m_Team == Team.Neutral)
        {
            //if NPC is (neutral) then all units that fit tag criteria above are potential targets EXCEPT other neutrals
            if (isTgtable && ut != Team.Neutral)
            {
                hostiles.Add(other.transform);
            }
        }
        else
        {

            // if this turret has a team, check that
            List<Transform> inRange = new List<Transform>();
            if (isTgtable && ut != m_Team)
            {
                // and the intruder is on a different team
                hostiles.Add(other.transform);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // When units die within the targeting range, a data ghost is left as null in the enemy units array. To fix this, regularly remove nulls
        for (int i = 0; i < hostiles.Count; i++)
        {
            Transform t = hostiles[i];
            if (t == null || t.gameObject.activeInHierarchy == false) // If destroyed OR disabled
                hostiles.Remove(t);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hostiles.Contains(other.transform))
        {
            hostiles.Remove(other.transform);
        }
    }

    public Transform RequestClosestTarget()
    {
        if (hostiles.Count == 0)
            return null;

            float dist_low = float.MaxValue;
            Transform close_targ = null;
            for (int i = 0; i < hostiles.Count; i++)
            {
                
            Transform t = hostiles[i];
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
