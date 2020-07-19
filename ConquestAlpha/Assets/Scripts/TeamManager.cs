using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Red,
    Blue,
    Neutral
}
public class TeamManager : MonoBehaviour
{
    public List<GameObject> blueUnits = new List<GameObject>();
    public List<GameObject> redUnits = new List<GameObject>();
    public List<Transform> blueBases = new List<Transform>();
    public List<Transform> redBases = new List<Transform>();
    public List<Transform> neutralBases = new List<Transform>();

    private void Start()
    {
        InvokeRepeating("SortTeamUnits", 0f, 2f);
    }
    public void SortTeamUnits()
    {

        GameObject[] allNPCs;
        allNPCs = GameObject.FindGameObjectsWithTag("NPC");

        if (allNPCs.Length == 0)
        {
            Debug.Log("No Units to Find!");
            return;
        }


        foreach (GameObject npc in allNPCs)
        {
            Team npcT = npc.GetComponent<NPCAI>().m_team;
            Debug.Log("NPC is team " + npcT);
            if (npcT == Team.Blue)
               if (!blueUnits.Contains(npc))
                    blueUnits.Add(npc);

            if (npcT == Team.Red)
                if (!blueUnits.Contains(npc))
                    redUnits.Add(npc);
        }
    }
    public GameObject[] FindTeamUnits(Team t)
    {
        GameObject[] allNPCs;
        allNPCs = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npc in allNPCs)
        {
            Team ut = npc.GetComponent<NPCAI>().m_team;
            if (ut == Team.Blue)

                blueUnits.Add(npc);

            if (ut == Team.Red)

                redUnits.Add(npc);
        }

        if (t == Team.Blue)
        {
            
            return blueUnits.ToArray();
        }

        if (t == Team.Red)
        {
            
            return redUnits.ToArray();
        }

        Debug.Log("No units exist for team " + t);
        return new GameObject[0];

    }
}
