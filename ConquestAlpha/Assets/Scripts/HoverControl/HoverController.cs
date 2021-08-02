using System;
using System.Collections;
using UnityEngine;
//Nathan Frazier
// https://www.youtube.com/watch?v=5B6ALcOX4b8
[RequireComponent(typeof(Rigidbody))]
public class HoverController : MonoBehaviour
{
    Rigidbody m_body;
    float m_deadZone = 0.1f;

    [SerializeField] public float m_fwardAcl = 100.0f;
    
    public float m_bwardAcl = 25.0f;
    float m_currThrust = 0.0f;
    float m_baseAclFWD;
    float m_baseAclBWD;
    public float m_boostSpeed = 3000f;
    public float m_turnStrength = 10f;
    float m_currTurn = 0.0f;
    Vector3 centerOMass;

    int m_layerMask;
    
    public float m_hoverForce = 9.0f;
    public float m_hoverHeight = 2.0f;
    public GameObject[] m_hoverPoints;
    private int m_currStrafe;
    bool isBoosting = false;
    private void Awake()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOMass;
        m_body = GetComponent<Rigidbody>();
        m_layerMask = 1 << LayerMask.NameToLayer("Characters");
        m_layerMask = ~m_layerMask;
        m_baseAclFWD = m_fwardAcl;
        m_baseAclBWD = m_bwardAcl;
    }

    private void Update()
    {
        // Input calculations

        //main thrust
        m_currThrust = 0.0f;
        float aclAxis = Input.GetAxis("Vertical");
        if (aclAxis > m_deadZone)
            m_currThrust = aclAxis * m_fwardAcl;
        else if (aclAxis < -m_deadZone)
            m_currThrust = aclAxis * m_bwardAcl;

        //turning
        m_currTurn = 0f;
        float turnAxis = Input.GetAxis("Mouse X");
        if (Mathf.Abs(turnAxis) > m_deadZone)
            m_currTurn = turnAxis;

        //strafing
        float strafeAxis = Input.GetAxis("Strafe");
        m_currStrafe = 0;

        if (Input.GetKey(KeyCode.D))
        {
            
            m_currStrafe = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            m_currStrafe = -1;
        }
        else
        {
            m_currStrafe = 0; // if neither E or Q, then not strafing.
        }

        // Boosting

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("Boostio!");
            isBoosting = true;
        } else
        {
            isBoosting = false;
        }
    }

    private void FixedUpdate()
    {
        // Physics calculations

        //IDEA: Raycast once and store wether or not each hover point hits. If it does, check to see if the length is <= hover height
        // * if it is <= hover height, then apply the force at position to that point
        //Hover Force
        RaycastHit hit;
        bool[] isDownward = new bool[m_hoverPoints.Length];

        for (int i = 0; i < m_hoverPoints.Length; i++)
        {
            var hoverPoint = m_hoverPoints[i];
            if (Physics.Raycast(hoverPoint.transform.position, -transform.up, out hit, m_layerMask) && hit.distance <= m_hoverHeight) { 
                m_body.AddForceAtPosition(transform.up.normalized * m_hoverForce * (1.0f - (hit.distance / m_hoverHeight)), hoverPoint.transform.position);
                m_body.drag = 2;
            } // if there is a hit and that same hit distance is <= hover height
            else
            {
                // this raycast is too high up off the ground
                m_currThrust = m_currThrust / 5; // keep some mid air control however reduce it by a lot.
                m_body.drag = 0.1f;
                isBoosting = false;
            }
        }

        //Forward
        if (Math.Abs(m_currThrust) > 0)
            m_body.AddForce(transform.forward * m_currThrust);

        //Turn
        if(m_currTurn > 0)
        {
            m_body.AddRelativeTorque(Vector3.up * m_currTurn * m_turnStrength);
        }else if (m_currTurn < 0)
        {
            m_body.AddRelativeTorque(Vector3.up * m_currTurn * m_turnStrength);
        }

        //Strafe

        if (m_currStrafe != 0)
        {
            //If player is strafing either A or D check the raycasts
            for (int i = 0; i < m_hoverPoints.Length; i++)
            {
                GameObject hoverPoint = m_hoverPoints[i];
                // if there is no hit on any one of the 4 then that means that corner is in the air technically
                // player should not be able to work against gravity by adding force sideways while on an angle
                if (!Physics.Raycast(hoverPoint.transform.position, -transform.up, out hit, m_hoverHeight, m_layerMask))
                    m_currStrafe = 0;

            }

            if (m_currStrafe == 1) // if D
            {
                    m_body.AddForce(transform.right * m_bwardAcl);
            }
            if (m_currStrafe == -1) // if A
            {
                
                m_body.AddForce(- transform.right * m_bwardAcl);
            }
        }

        // If boosting, then buff the speed over time. If not boosting then return to normal values.
        if (isBoosting)
        {
            //forward acl
            if (m_fwardAcl < m_baseAclFWD + m_boostSpeed)
            {
                m_fwardAcl += Time.deltaTime * m_boostSpeed;
            }

            // reverse acl + strafe acl
            if (m_bwardAcl < m_baseAclBWD + m_boostSpeed)
            {
                m_bwardAcl += Time.deltaTime * m_boostSpeed;
            }

        } else if (m_fwardAcl > m_baseAclFWD && m_bwardAcl > m_baseAclBWD)
        {
            //Decays when input is not w a s d shift
            m_fwardAcl -= Time.deltaTime * m_boostSpeed;
            m_bwardAcl -= Time.deltaTime * m_boostSpeed;
        }
    }

    void OnDrawGizmos()
    {
        
    }
}
