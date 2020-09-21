using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerTA : MonoBehaviour
{
    
    // Player Targeting Agent
    
    public Image lockOnUI;
    public Image reticle;
    int h = 0; // always start 0
    static Transform[] playerTforms;
    static List<PlayerTarget> [] playerInRanges;
    public List<PlayerTarget> inRange;
    [Header("Crosshair Auto Lock Parameters")]
    public static int lockRange = 50;
    
    [SerializeField] PlayerTarget currTarget;
    PlayerTarget clockwise;
    TeamManager myTM;

    // make an activate + deactivate method for the lock on and have them erase the Q when turned off

    void Awake()
    {  
        inRange = new List<PlayerTarget>();
        int pcount = GameManager.CountPlayers();
        Debug.Log("Pcount = " + pcount);
        playerInRanges = new List<PlayerTarget>[pcount];
        playerInRanges[pcount - 1] = new List<PlayerTarget>(); // HARD CODED FOR ONE PLAYER

        currTarget = null;


        myTM = GetComponent<TeamManager>();
        Team myTeam = myTM.m_Team;
        InvokeRepeating("TargetingTick", 0f, 0.05f);
    }


    public static void MovedIntoRange(int playerNum, PlayerTarget target)
    {
        List<PlayerTarget> tmpIR = playerInRanges[playerNum - 1];

        if (!tmpIR.Contains(target))
        {
            Debug.Log("ADDING target " + target.name + " to the in range list of player " + playerNum);
            tmpIR.Add(target);

        }
        // Add new target to inRange. This is going to be an enemy team target.
        
        playerInRanges[playerNum - 1] = tmpIR; // save changes to static
    }
    public static void OutOfRange(int playerNum, PlayerTarget target)
    {
        List<PlayerTarget> tmpIR = playerInRanges[playerNum - 1];
        if (tmpIR.Contains(target))
        {
            Debug.Log("REMOVING target " + target.name + " from the in range list of player " + playerNum);
            tmpIR.Remove(target);
        }
        
        // Add new target to inRange. This is going to be an enemy team target.

        playerInRanges[playerNum - 1] = tmpIR; // save changes to static
    }

    void TargetingTick()
    {
        
        //Sync the player number because its not correct on load time.
        h = GetComponent<PlayerManager>().playerNumber;
        
        // Viewport Point Z is negative if the point is located behind the viewport
        inRange = playerInRanges[h - 1];
        
        // NOW if our current target is null, assign the closest that is within the snap range.
        // first we need to add a PlayerTarget only if the enemy meets the screen space requirement
        if (currTarget == null)
        {
            float closest = float.MaxValue;
            PlayerTarget newTarget = null;
            for(int i = 0; i < inRange.Count; i++)
            {


                PlayerTarget pt = inRange[i];
                if (pt.pdist[h - 1] < closest)
                {
                    closest = pt.pdist[h-1];
                    newTarget = pt;
                    inRange.Remove(currTarget);
                }
                currTarget = newTarget;
            }
            
        } else
        {
            // If we have a current target add the others that are not current to the queue
            

        }
    }

    void AimGuns(Transform t)
    {
        // Need direction to the target
        //Debug.Log("Aim Dem Guns");

        GetComponent<PlayerManager>().primaryBulletSpawn.LookAt(t.position);
        GetComponent<PlayerManager>().secondaryBulletSpawn.LookAt(t.position);

    }
    void Update()
    {
        if (inRange.Count == 0) // update the position of the lockon UI element
        {
            lockOnUI.enabled = false;
            currTarget = null;
            GetComponent<PlayerManager>().primaryBulletSpawn.rotation = transform.rotation;
            GetComponent<PlayerManager>().secondaryBulletSpawn.rotation = transform.rotation;
        }
        else if (currTarget != null)
        {
            lockOnUI.enabled = true;
            lockOnUI.transform.position = currTarget.FindUIPosition();
            AimGuns(currTarget.transform);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //ROUTINE
            Debug.Log("TAB!");

        }
    }

}
