using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTA : MonoBehaviour
{
    
    // Player Targeting Agent
    
    public Image lockOnUI;
    public Image reticle;
    
    int h = 0; // always start 0
    public List<GameObject> inRange;
    [Header("Crosshair Auto Lock Parameters")]
    public float lockRange = 50f;
    public float spherecast_r = 10f;
    public float sweetspot = 0.10f; // in viewport space on scale from 0 left to 1 right
    TeamManager myTM;
    public Camera pcam;
    RaycastHit camRay;
    public GameObject aimIndicator;
    // make an activate + deactivate method for the lock on and have them erase the Q when turned off

    void Awake()
    {
        myTM = gameObject.GetComponent<TeamManager>();
        inRange = new List<GameObject>();
        InvokeRepeating("TargetingTick", 0.5f, 0.15f);
    }


    public void MovedIntoRange(GameObject tgt)
    {
        inRange.Add(tgt);
        inRange.TrimExcess();

    }
    public void OutOfRange(GameObject t)
    {
        inRange.Remove(t);
        inRange.TrimExcess();
    }

    void TargetingTick()
    {
        //Debug.DrawRay(pcam.transform.position, pcam.transform.forward, Color.cyan, 0.5f);
        if (Physics.SphereCast(pcam.transform.position, spherecast_r, pcam.transform.forward, out camRay, lockRange))
        {
            //Debug.Log("Spherecast hit something!");
            TeamManager tm;
            aimIndicator.transform.position = camRay.point;
            if (camRay.transform.gameObject.TryGetComponent<TeamManager>(out tm))
            {

                if (!tm.m_Team.Equals(myTM.m_Team) && !inRange.Contains(camRay.transform.gameObject))
                {
                    // Dont add if its ouside of the sweet spot
                    var viewportPt = Camera.main.WorldToViewportPoint(camRay.transform.position);
                    if (viewportPt.x < 0.5f - sweetspot || viewportPt.x > 0.5f + sweetspot)
                        return;
                    Debug.Log("Enemy Detected");
                    MovedIntoRange(camRay.transform.gameObject);
                }

            }
        }

        for (int i = 0; i < inRange.Count; i++)
        {
            Renderer r = inRange[i].gameObject.GetComponentInChildren<Renderer>();

            var viewportPt = Camera.main.WorldToViewportPoint(inRange[i].transform.position);
            //Debug.Log(viewportPt + inRange[i].name);

            if (viewportPt.x < 0.5f - sweetspot || viewportPt.x > 0.5f + sweetspot)
            {
                Debug.Log("Moved out of sweetspot");
                OutOfRange(inRange[i]);
            }
            else if (!r.isVisible || Vector3.Distance(gameObject.transform.position, inRange[i].transform.position) > lockRange)
            {
                Debug.Log("Target became not visible or too far away");
                OutOfRange(inRange[i]);
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

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //ROUTINE
                Debug.Log("TAB!");

            }
        }
    }
}
