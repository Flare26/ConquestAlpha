﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Nathan Frazier
public enum Team
{
    Red,
    Blue,
    Neutral
}

public class TeamManager : MonoBehaviour
{

    public static List<GameObject> redTeam;
    public static List<GameObject> blueTeam;
    public Light[] teamLights;
    public Team m_Team;
    [SerializeField] public bool isTgtable = true;
    [HideInInspector]public Renderer mesh = null;
    void Awake()
    {
        //Initialize static lists
        redTeam = new List<GameObject>();
        blueTeam = new List<GameObject>();
    }

    private void OnEnable()
    {
        AssignTeam(m_Team);
    }
    public void AssignTeam(Team t)
    {
        Debug.Log("AssignTeam --> " + t);
        // Each time a team is assigned, it will check the static list
        switch (m_Team)
        {
            case Team.Red:
                if (blueTeam.Contains(this.gameObject))
                {
                    blueTeam.Remove(this.gameObject);
                    redTeam.Add(this.gameObject);
                }
                break;

            case Team.Blue:
                if (redTeam.Contains(this.gameObject))
                {
                    redTeam.Remove(this.gameObject);
                    blueTeam.Add(this.gameObject);
                }
                break;
        }

        m_Team = t;
        NPCTargetingAgent agt;
        if (TryGetComponent<NPCTargetingAgent>(out agt))
        {
            agt.SetTeam(t);
        }
        TeamColorsUpdate();
    }
    
    public Color SetMeshColor()
    {
        Color c = Color.white;

        switch (m_Team)
        {
            case Team.Red:
                c = Color.red;
                break;
            case Team.Blue:
                c =  Color.blue;
                break;
            case Team.Neutral:
                c = Color.green;
                break;
        }
        
        return c;
    }

    void SetTeamLights()
    {
        for (int i = 0; i < teamLights.Length; i++)
        {
            Light glow = teamLights[i];
            switch (m_Team)
            {
                case Team.Neutral:
                    glow.color = Color.green;
                    break;
                case Team.Blue:

                    glow.color = Color.blue;
                    break;
                case Team.Red:

                    glow.color = Color.red;
                    break;
            }
        }
    }
    void TeamColorsUpdate()
    {
        if (!mesh.Equals(null))
            mesh.material.color = SetMeshColor();
        SetTeamLights();
    }
}
