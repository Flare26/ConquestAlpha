using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// these are going to be created and built with the team of the base
public class NPCAI : MonoBehaviour
{

    [SerializeField]
    Transform destination;
    NavMeshAgent navAgent;

    string team = "Neutral";
    int hull;
    double shields;
    public float shieldRechargeRate = 4f; // 4sec
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.Log("NAV ERROR: Please attach NavMeshAgent to " + gameObject.name);
        }
        else
            SetDestination();
    }

    private void SetDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navAgent.SetDestination(targetVector);
        }
    }
}

    // Update is called once per frame

