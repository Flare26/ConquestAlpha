using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretConstructor : MonoBehaviour
{
    int capacity = 4; // How many turrets a base can have
    bool[] constructed = new bool[4]; // Each turret has an entry. Array stores whether or not turret is alive or dead / unconstructed. A dead turret = unconstruct
    
    public GameObject turretPlate1;
    public GameObject turretPlate2;
    public GameObject turretPlate3;
    public GameObject turretPlate4;
    public float buildProgress = 0; // seconds
    public float buildTime = 5f; //seconds
    public float buildPercent; // percentage
    public bool isDocked;
    public string team;
    Queue<GameObject> buildQueue;
    bool[] built;
    Transform[] allChildren;

    // Start is called before the first frame update
    void Start()
    {
        team = "neutral";
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
            Debug.Log("Player has docked");
            isDocked = true;
            // If build percent is 0 at the time a player docks...
            if (buildPercent == 0)
            {
                team = other.gameObject.GetComponent<ThirdpersonCharController>().team; // Set base on the same team as player
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Player is docked");
            // Check if build progress is under 100%
            if (buildProgress < buildTime)
            {
                buildProgress += Time.deltaTime; // Count total number of seconds the player has been in the dock zone
                
            }
            this.buildPercent = ((buildProgress) / (buildTime)) * 100; // Convert
            if (buildProgress > buildTime)
            {
                buildProgress = buildTime;
                buildPercent = 100;
            }
            
            //Debug.Log("Built % = " + (buildProgress * 100) / (buildTime * 100));
        }
        // end player collision check
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has un-docked " );
            isDocked = false;
        }
    }
    void BuildTurret(int idx)
    {
        if (!built[idx])
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
