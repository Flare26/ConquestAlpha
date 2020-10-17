using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] players;
    public int preptime;
    public int maxtime;
    public float matchtime = 0f;

    // Start is called before the first frame update


    private void Awake()
    {
        matchtime -= preptime;
    }

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

    void StartGame()
    {
        Debug.Log("The Game Has Started!!");
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

    private void Update()
    {
        matchtime += Time.deltaTime;
        if (matchtime > 0)
        {
            StartGame();
        }
    }

}
