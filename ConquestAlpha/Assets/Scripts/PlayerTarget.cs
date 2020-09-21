using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
//Nathan Frazier
public class PlayerTarget : MonoBehaviour
{
    
    public Transform[] playerTforms;
    // make jagged array that stores arrays of targets within player's range
    TeamManager myTM;
    public float [] pdist;
    // Update is called once per frame
    private void Awake()
    {
        // All units with a PlayerTarget should know where each player is at all times using the static array.
        var gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        playerTforms = new Transform[GameManager.CountPlayers()];
        playerTforms = GameManager.GetAllPlayerTforms();

        pdist = new float[playerTforms.Length];
        myTM = GetComponentInParent<TeamManager>();
        InvokeRepeating("TargetingTick", 4f, 0.35f);
    }

    public Vector3 FindUIPosition()
    {
        var myViewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        var myScreenPoint = Camera.main.ViewportToScreenPoint(myViewportPoint);
        return myScreenPoint; // set the lock on cursor to follow me!
    }
    public void TargetingTick()
    {
        // For each player, calculates the distance from THIS UNIT to PLAYER 1,2,3.... 
        // if this unit is under the required distance from any given player, add its player target to the list of targets to lock on to
        for (int i = 0; i < playerTforms.Length; i++)
        {
            Transform player = playerTforms[i];
            pdist[i] = Vector3.Distance(transform.position, player.position);
            
            Vector3 myViewportPoint = Camera.main.WorldToViewportPoint(this.transform.position);
            //Debug.Log("Viewport X = " + myViewportPoint.x);
            if (pdist[i] > PlayerTA.lockRange)
            {
                //Debug.Log("Player too far from me!");
                // Call moved out of range method and pass this guy :)
                PlayerTA.OutOfRange(player.GetComponent<PlayerManager>().playerNumber, GetComponent<PlayerTarget>());
            }
            else if (pdist[i] < PlayerTA.lockRange && myViewportPoint.z > 0)
            {
                //Debug.Log("Player close enough!");
                List<PlayerTarget> inPlayersRange = player.GetComponent<PlayerTA>().inRange;
                Team myTeam = myTM.m_Team;
                Team playerTeam = player.GetComponent<TeamManager>().m_Team;
                //Debug.Log(myTeam + " vs " + playerTeam);
                if (inPlayersRange.Contains(GetComponent<PlayerTarget>()))
                        return;
                
                    if (!myTeam.Equals(playerTeam))
                    {
                        //Debug.Log("Moving this targ into player range .... " + gameObject.name );
                        PlayerTA.MovedIntoRange(player.GetComponent<PlayerManager>().playerNumber, GetComponent<PlayerTarget>());
                    }
                
                    
            } else
            {
                //Debug.Log(myViewportPoint);
                PlayerTA.OutOfRange(player.GetComponent<PlayerManager>().playerNumber, GetComponent<PlayerTarget>());
            }


        }
        // End the update in range process
      
    }
}
