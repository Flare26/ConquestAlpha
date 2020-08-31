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

    public static List<Transform> redUnits;
    public static List<Transform> redTurrets;
    public static List<Transform> blueUnits;
    public static List<Transform> blueTurrets;
}
