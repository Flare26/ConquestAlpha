using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Nathan Frazier
public abstract class GameUnit : MonoBehaviour
{
    enum State
    {
        AttackHQ,
        DefendHQ,
        DefendPost,
        HoldGround
    }

    // Start is called before the first frame update


    [Header("Base Stats")]
    // Basic Stats
    public int h_current;
    public int h_max;
    public int sh_current;
    public int sh_max;
    public int sh_rate;
    public int sh_delay;
    public float range = 20f;


    // Weaponry
    [Header("Weaponry")]
    [SerializeField] public GameObject primaryWep;
    [SerializeField] public GameObject secondaryWep;
    [HideInInspector] public GameObject primaryInstance;
    [HideInInspector] public GameObject secondaryInstance;

    [Header("Backend")]
    // Backend
    [SerializeField] State stance;
    public Transform mount_Primary;
    public Transform mount_Secondary;
    [SerializeField] public Transform p_FP; // primary fire point
    [SerializeField] public Transform s_FP; // secondary fire point
    public List<GameObject> targetedBy;
    public bool hasShield;

    private void OnEnable()
    {
        // On enable set the correct team color
        targetedBy = new List<GameObject>();


        mount_Primary = transform.GetChild(0).transform;
        mount_Secondary = transform.GetChild(2).transform;

        if (primaryWep != null)
        {
            primaryInstance = Instantiate<GameObject>(primaryWep, mount_Primary.position, this.transform.rotation);
        }
        // Secondary wep not accounted for yet

        // in the prefab the mount points should be first 2 children on the npc
        // The target sphere should always be the last

        NavMeshAgent navAgent;
        if (TryGetComponent<NavMeshAgent>(out navAgent))
        {
            Debug.Log("NavMeshAgent Found! ");
        }else
        {
            Debug.Log("NO NavMeshAgent on object: " + gameObject.name);
        }
    }

}
