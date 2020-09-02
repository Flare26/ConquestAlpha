using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPost : MonoBehaviour
{
    //public GameObject curr_builder;
    public string ownerName;
    public float completionTime; //seconds
    public float timeSpentBuilding = 0f;
    public bool isDocked;
    public bool producingUnits;
    GameObject inProgress;
    TeamManager tm;
    //public string team;
    public Queue<GameObject> turretQ;
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize
        turretQ = new Queue<GameObject>();
        Transform [] allChildren = GetComponentsInChildren<Transform>();
        tm = GetComponent<TeamManager>();

        //Enqueue all except for what base spawns with
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Turret"))
            {
                child.gameObject.SetActive(false);
                turretQ.Enqueue(child.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in a base");
            isDocked = true;
        }    
    }

    private void OnTriggerStay(Collider other)
    {
        TeamManager ptm;
        if (other.CompareTag("Player") && other.gameObject.TryGetComponent<TeamManager>(out ptm))
        {
            timeSpentBuilding += Time.deltaTime;

            if (timeSpentBuilding >= completionTime && turretQ.Count > 0)
            {
                tm.m_Team = ptm.m_Team; // set the team for the command post equal to the team which the player is on, should they stay to build something
                timeSpentBuilding = 0;
                BuildNextTurret(ptm);
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (!other.CompareTag("Player"))
            return;

        if (timeSpentBuilding < completionTime)
        {
            timeSpentBuilding = 0;
            Debug.Log("Player has un-docked prematurely!");
        }
        
        isDocked = false;

    }

    void BuildNextTurret(TeamManager ptm)
    {
        Debug.Log("Building From Q");
        GameObject buildNext = turretQ.Dequeue();
        buildNext.SetActive(true);
        buildNext.GetComponent<TeamManager>().AssignTeam(tm.m_Team);
    }

    void BuildNextUnit()
    { 
    }
}
