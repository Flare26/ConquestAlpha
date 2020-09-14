using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Nathan Frazier
public class PlayerTarget : MonoBehaviour
{
    public Image LockOn;
    static Transform[] playerTforms;
    int playerLockRange = 20;
    bool isNearPlayer = false;
    float[] pdist;
    // Update is called once per frame
    private void Awake()
    {
        // All units with a PlayerTarget should know where each player is at all times using the static array.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playerTforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerTforms[i] = players[i].transform; // assign the transforms
        }

        pdist = new float[players.Length];
    }
    void FixedUpdate()
    {
        int maxDist = 50; // HARD WIRED FOR NOW
        var distFromPlayer = Vector3.Distance(transform.position, playerTforms[0].position); // HARD WIRED FOR NOW
        //Debug.Log("Dist from player = " + distFromPlayer);
        if (distFromPlayer > maxDist) // if the player is out of targeting range disable the lock on image
        {
            LockOn.enabled = false;
        } else
        {
            // in here we can access a static array of players and depending on the distance to this given unit, determine wether or not to lock on
            //if player already has a target though dont even bother
            if (LockOn.enabled == false)
                LockOn.enabled = true;

            Vector3 reticlePos = Camera.main.WorldToScreenPoint(this.transform.position);
            LockOn.transform.position = reticlePos;
        }
    }

    private void OnDestroy()
    {
        //on destroy, check the potential target Q for the player. If there are no more targets in LoS, return to center
        LockOn.enabled = false;
    }
}
