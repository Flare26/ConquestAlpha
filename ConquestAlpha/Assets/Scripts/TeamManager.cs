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

    public static List<Transform> redTeam;
    public static List<Transform> blueTeam;
    public Team m_Team;

}
