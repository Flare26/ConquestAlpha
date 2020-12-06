using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Nathan Frazier
public class CommandPost : MonoBehaviour
{
    //public GameObject curr_builder;
    LatticeLine lattice;
    public bool isCappable;
    public string ownerName;
    public float completionTime; //seconds
    public float timeSpentBuilding = 0f;
    public bool isDocked;
    public bool producingUnits;
    public GameObject[] spawnables;
    public GameObject[] turrets;
    GameObject inProgress;
    TeamManager tm;
    //public string team;
    public Queue<GameObject> turretQ;
    public Queue<GameObject> spawnableQ;
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize
        turretQ = new Queue<GameObject>();
        spawnableQ = new Queue<GameObject>();
 
        tm = GetComponent<TeamManager>();

        foreach (GameObject g in spawnables)
        {
            spawnableQ.Enqueue(g);
        }

        //Enqueue all except for what base spawns with
        foreach (GameObject child in turrets)
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
        // When there is a trigger enter, refresh the isCappable
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
        buildNext.GetComponent<TeamManager>().AssignTeam(tm.m_Team); // set team of the turret to the command post team
    }

    void BuildNextUnit()
    {
        GameObject buildNext = spawnableQ.Dequeue();
        buildNext.SetActive(true);
        buildNext.GetComponent<TeamManager>().AssignTeam(tm.m_Team); // set team of the turret to the command post team
    }
    private void Update()
    {
        
    }
}
