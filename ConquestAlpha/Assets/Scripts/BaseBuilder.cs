using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuilder : MonoBehaviour
{
    int capacity = 4; // How many turrets a base can have
    bool[] constructed = new bool[4]; // Each turret has an entry. Array stores whether or not turret is alive or dead / unconstructed. A dead turret = unconstruct
    
    //public GameObject curr_builder;
    public string ownerName;
    public float secondsInside = 0; // seconds
    public float turretBuildTime; //seconds
    public float timeSpentBuilding = 0f;
    [SerializeField] float spawnableTime = 5f;
    public int cTurretsAlive; // percentage
    public bool isDocked;
    public bool producingUnits;

    //public string team;
    Queue<GameObject> turretQ;
    Queue<GameObject> spawnableQ;
    [SerializeField] List<GameObject> aliveUnits;
    bool[] built_turrets;
    bool[] built_spawnables;
    Transform[] allChildren;
    [SerializeField] GameObject[] npcSpawnablesLoadout;
    public Team m_Team;
    
    public float timeSinceLastSpawnable = 0;
    // Start is called before the first frame update
    void Awake()
    {
        m_Team = Team.Neutral;
        built_turrets = new bool [] {false, false, false, false};
        built_spawnables = new bool[] { false, false };
        turretQ = new Queue<GameObject>();
        spawnableQ = new Queue<GameObject>();
        allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.tag == "Turret")
            {
                child.gameObject.SetActive(false);
                turretQ.Enqueue(child.gameObject);
            }
        }
        aliveUnits = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        Debug.Log("A player has docked somwehere");
        
        m_Team = other.GetComponent<TeamManager>().m_Team;
            


            isDocked = true;
            // If build percent is 0 at the time a player docks...
        


    }
    private void OnTriggerExit(Collider other)
    {

        if (!other.CompareTag("Player"))
            return;
        
        Debug.Log("Player has un-docked ");
        isDocked = false;
        timeSpentBuilding = 0;
        
    }

    void BuildTurret(Queue<GameObject> turretQ)
    {
        Debug.Log("Turret built from queue");
        GameObject currentTurret = turretQ.Dequeue();
        currentTurret.SetActive(true);
    }

    IEnumerator CreateSpawnable(GameObject [] spawnables)
    {
        int idx = -1;

        // wait 5 sec, then check to find the first built spawnable index that = false. take the index and instantiate that index from the spawnables array
        // this system will hopefully allow for the player to change what units their bases spawn in the future
        for (int i = 0; i < npcSpawnablesLoadout.Length; i++)
        {
            if (built_spawnables[i] == false)
            {
                built_spawnables[i] = true;
                spawnableQ.Enqueue(spawnables[i]);
            }
        }

        yield return new WaitForSeconds(5f);

        if (spawnableQ.Count > 0)
        {
            GameObject thingtospawn = spawnableQ.Dequeue();
            var instance = Instantiate(thingtospawn, transform.position, transform.rotation);
            aliveUnits.Add(instance);

            // make sure the unit is removed from hostiles. since it is briefly neutral upon making.
            foreach (TargetingAgent ta in GetComponentsInChildren<TargetingAgent>())
            {
                if (ta.hostiles.Contains(instance.transform))
                    ta.hostiles.Remove(instance.transform);
            }
        }



        Debug.Log("A unit has been created");
    }

    // Update is called once per frame
    void Update()
    {
        if (cTurretsAlive < 4)
            producingUnits = false;

        if (producingUnits)
        {
            if (timeSinceLastSpawnable > spawnableTime && aliveUnits.Count < npcSpawnablesLoadout.Length)
            {
                Debug.Log("Began production on a unit");
                StartCoroutine(CreateSpawnable(npcSpawnablesLoadout));
                timeSinceLastSpawnable = 0f;
            }
        }

        if (isDocked == true)
        {
            
            // Debug.Log("Player is docked");
            // Check if build progress is under 100%
            if (cTurretsAlive < 4)
            {
                timeSpentBuilding += Time.deltaTime; // Count total number of seconds the player has been in the dock zone
            }

            if ( timeSpentBuilding >= turretBuildTime && turretQ.Count > 0)
            {
                Debug.Log("Releasing turret from turretQ");
                timeSpentBuilding = 0;
                cTurretsAlive++;
                BuildTurret(turretQ);
            }
            
            if (turretQ.Count == 0 && producingUnits == false)
            {
                producingUnits = true;
            }
        } // end isDocked == true
        timeSinceLastSpawnable += Time.deltaTime;
    }
}
