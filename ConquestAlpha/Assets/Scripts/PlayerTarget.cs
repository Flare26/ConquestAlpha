using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Nathan Frazier
public class PlayerTarget : MonoBehaviour
{
    public Image LockOn;
    static Transform[] playerTforms;
    static Image reticle;
    int playerLockRange = 20;
    bool isNearPlayer = false;
    float[] pdist;
    // Update is called once per frame
    private void Awake()
    {
        // All units with a PlayerTarget should know where each player is at all times using the static array.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        reticle = GameObject.Find("Reticle").GetComponent<Image>();
        playerTforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerTforms[i] = players[i].transform; // assign the transforms
        }

        pdist = new float[players.Length];
    }
    void FixedUpdate()
    {
        // Viewport Point Z is negative if the point is located behind the viewport

        int maxDist = 50; // HARD WIRED FOR NOW
        var distFromPlayer = Vector3.Distance(transform.position, playerTforms[0].position); // HARD WIRED FOR NOW
        Vector3 myViewportPoint = Camera.main.WorldToViewportPoint(this.transform.position);
        //Debug.Log("myViewportPoint = " + myViewportPoint);
        Debug.Log(playerTforms[0].rotation);
        bool isSnapped = false;
        if (myViewportPoint.x < 0.6f && myViewportPoint.x > 0.40f && myViewportPoint.z > 0 )
            isSnapped = true;
        if (distFromPlayer > maxDist || !isSnapped) // if the player is out of targeting range disable the lock on image
        {
            LockOn.enabled = false;
        } else
        {
            // in here we can access a static array of players and depending on the distance to this given unit, determine wether or not to lock on
            //if player already has a target though dont even bother
            if (LockOn.enabled == false)
                LockOn.enabled = true;
            var myScreenPoint = Camera.main.ViewportToScreenPoint(myViewportPoint);
            LockOn.transform.position = myScreenPoint ; // set the lock on cursor to follow me!
        }
    }

    private void OnDestroy()
    {
        //on destroy, check the potential target Q for the player. If there are no more targets in LoS, return to center
        LockOn.enabled = false;
    }
}
