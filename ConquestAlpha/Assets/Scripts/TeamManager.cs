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

    public List<Transform> blueBases = new List<Transform>();
    public List<Transform> redBases = new List<Transform>();

    public List<Transform> neutralBases = new List<Transform>();
}
