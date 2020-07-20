using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TurretTargeter : MonoBehaviour
{
    // This script at it's core should search for a suitable target in range for the turret that it is a parent of. It should present the turret with a target every time it requests one.
    // Made to be used with a targetArea collision trigger sphere or object 
    [SerializeField] public List<Transform> hostiles;
    [SerializeField] public Team m_Team;
    Team e_Team; // If turret is neutral, this value remains NULL
    private void OnEnable()
    {
        var tAI = GetComponentInParent<NewTurretAI>();
        GetComponent<SphereCollider>().radius = tAI.range; // set the range collider to proper size based on the turret range stat
        m_Team = tAI.m_Team;
        
        
    }
    private void OnTriggerEnter(Collider other)
    {
        var tmp = hostiles;
        bool isNPC = false;
        Team ut = Team.Neutral;
        if (other.tag == "NPC")
        {
            ut = other.GetComponent<NPCAI>().m_team;
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
        } else
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

    private void OnTriggerStay(Collider other)
    {
        // When units die within the targeting range, a data ghost is left as null in the enemy units array. To fix this, regularly remove nulls
        for (int i = 0; i < hostiles.Count; i++)
        {
            Transform t = hostiles[i];
            if (t == null)
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
            float dist_low = float.MaxValue;
            Transform close_targ = null;
            for (int i = 0; i < hostiles.Count; i++)
            {
            
                Transform t = hostiles[i];
                if (t == null)
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
