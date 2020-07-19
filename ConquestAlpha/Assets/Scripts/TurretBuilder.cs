using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBuilder : MonoBehaviour
{
    int capacity = 4; // How many turrets a base can have
    bool[] constructed = new bool[4]; // Each turret has an entry. Array stores whether or not turret is alive or dead / unconstructed. A dead turret = unconstruct
    
    public GameObject turretPlate1;
    public GameObject turretPlate2;
    public GameObject turretPlate3;
    public GameObject turretPlate4;
    private GameObject owner;
    public string ownerName;
    public float secsBuilt = 0; // seconds
    public float buildTime = 5f; //seconds
    public float buildPercent; // percentage
    public bool isDocked;
    //public string team;
    Queue<GameObject> buildQueue;
    bool[] built;
    Transform[] allChildren;
    public Team m_Team;
    // Start is called before the first frame update
    void Start()
    {
        m_Team = Team.Neutral;
        built = new bool [] {false, false, false, false};
        buildQueue = new Queue<GameObject>();
        allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.tag == "Turret")
            {
                child.gameObject.SetActive(false);
                buildQueue.Enqueue(child.gameObject);
            }
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has docked somwehere");
            ownerName = other.gameObject.name;
            if (m_Team == Team.Neutral)
                m_Team = other.GetComponent<ThirdpersonCharController>().m_Team;
            


            isDocked = true;
            // If build percent is 0 at the time a player docks...
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Player is docked");
            // Check if build progress is under 100%
            if (secsBuilt < buildTime)
            {
                secsBuilt += Time.deltaTime; // Count total number of seconds the player has been in the dock zone
                
            }
            this.buildPercent = ((secsBuilt) / (buildTime)) * 100; // Convert
            if (secsBuilt > buildTime)
            {
                secsBuilt = buildTime;
                buildPercent = 100;
                owner = other.gameObject; // This player is now the owner
                ownerName = other.gameObject.name;
            }
            
            //Debug.Log("Built % = " + (buildProgress * 100) / (buildTime * 100));
        }
        // end player collision check
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has un-docked ");
            isDocked = false;
        }

        if (buildPercent >= 0 && buildPercent < 25)
        {
            secsBuilt = 0;
            buildPercent = 0;
        }
        else

        if (buildPercent > 25 && buildPercent < 50)
        {
            secsBuilt = 0.25f * buildTime;
            buildPercent = 25;
        }
        else

        if (buildPercent > 50 && buildPercent < 75)
        {
            secsBuilt = 0.5f * buildTime;
            buildPercent = 50;
        }
        else

        if (buildPercent > 75 && buildPercent < 100)
        {
            secsBuilt = 0.75f * buildTime;
            buildPercent = 75;
        }
        else
            return;
    }
    void BuildTurret(int idx)
    {
        if (!built[idx]) // if not built currently
        {
            Debug.Log("Turret"+ idx + "built");
            GameObject currentTurret = buildQueue.Dequeue();
            currentTurret.SetActive(true);
            built[idx] = true;
        }

    }

    IEnumerator WaitThenBuild()
    {
        for (int i = 0; i < allChildren.Length; i++)
        {
            if (this.built[i] == false)
            {
                yield return new WaitForSeconds(2);
                Debug.Log("2 seconds elapsed");
                this.built[i] = true;
                allChildren[i].gameObject.SetActive(true);
            }

        }
    }
    void DestroyTurret(int idx)
    {
        built[idx] = false;
        allChildren[idx].gameObject.SetActive(false);
        secsBuilt -= 2.5f;

    }
    // Update is called once per frame
    void Update()
    {
        int simplePercent = Mathf.RoundToInt(buildPercent);
        //Debug.Log("Rounded Percent" + simplePercent);
        if (simplePercent == 25)
            BuildTurret(0);
        if (simplePercent == 50)
            BuildTurret(1);
        if (simplePercent == 75)
            BuildTurret(2);
        if (simplePercent == 100)
            BuildTurret(3);
    }
}
