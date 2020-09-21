using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] players;
    // Start is called before the first frame update
    public static int CountPlayers()
    {
        int slot = 1;

        GameObject [] s = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in s)
        {
            var pm = player.GetComponent<PlayerManager>();
            pm.playerNumber = slot;
            slot ++;
        }

        int playerCount = s.Length;

        return playerCount;
    }

    public static Transform [] GetAllPlayerTforms()
    {
        var a = GameObject.FindGameObjectsWithTag("Player");
        Transform[] b = new Transform[a.Length];
        for(int i = 0; i < a.Length; i++)
        {
            b[i] = a[i].transform;
        }
        return b;
    }

}
