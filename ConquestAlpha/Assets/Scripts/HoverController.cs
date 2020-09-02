using System;
using System.Collections;
using UnityEngine;
// https://www.youtube.com/watch?v=5B6ALcOX4b8
[RequireComponent(typeof(Rigidbody))]
public class HoverController : MonoBehaviour
{
    Rigidbody m_body;
    float m_deadZone = 0.1f;

    [SerializeField] public float m_forwardAcl = 100.0f;
    
    public float m_backwardAcl = 25.0f;
    float m_currThrust = 0.0f;
    float m_baseAcl;
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
    
    Transform boostFXinstance;

    [Header("Speedometer")]
    [SerializeField] float totalAcl = 0f;
    private void Awake()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOMass;
        m_body = GetComponent<Rigidbody>();
        m_layerMask = 1 << LayerMask.NameToLayer("Characters");
        m_layerMask = ~m_layerMask;
        m_baseAcl = m_forwardAcl;
        
    }

    private void Update()
    {
        // Input calculations

        //main thrust
        m_currThrust = 0.0f;
        float aclAxis = Input.GetAxis("Vertical");
        if (aclAxis > m_deadZone)
            m_currThrust = aclAxis * m_forwardAcl;
        else if (aclAxis < -m_deadZone)
            m_currThrust = aclAxis * m_backwardAcl;

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
            Debug.Log("Boostio!");

            isBoosting = true;
        } else
        {
            isBoosting = false;
        }
    }

    private void FixedUpdate()
    {
        totalAcl = GetComponent<Rigidbody>().velocity.magnitude;
        //Debug.Log("VELOCITY " + m_absVelocity);
        // Physics calculations
        //Hover Force
        RaycastHit hit;
        for (int i = 0; i < m_hoverPoints.Length; i++)
        {
            var hoverPoint = m_hoverPoints[i];
            if (Physics.Raycast(hoverPoint.transform.position, -Vector3.up, out hit, m_hoverHeight, m_layerMask))
                m_body.AddForceAtPosition(Vector3.up * m_hoverForce * (1.0f - (hit.distance / m_hoverHeight)), hoverPoint.transform.position);
            else
            {
                m_body.AddForceAtPosition(Vector3.down * 9.8f * 10, hoverPoint.transform.position);
                // Self corrects the vehicle based on which side is higher
                //if (transform.position.y > hoverPoint.transform.position.y)
                //    m_body.AddForceAtPosition(hoverPoint.transform.up * m_hoverForce, hoverPoint.transform.position);
                //else
                //    m_body.AddForceAtPosition(hoverPoint.transform.up * -m_hoverForce, hoverPoint.transform.position);
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
            if (m_currStrafe == 1)
            {
                
                m_body.AddForce(transform.right * m_backwardAcl);
            }
            if (m_currStrafe == -1)
            {
                
                m_body.AddForce(- transform.right * m_backwardAcl);
            }
        }

        // boost
        if (isBoosting)
        {
            if (m_forwardAcl < m_baseAcl + m_boostSpeed)
            {
                m_forwardAcl += Time.deltaTime * m_boostSpeed;
            }
        } else if (m_forwardAcl > m_baseAcl)
        {
            m_forwardAcl -= Time.deltaTime * m_boostSpeed;
        }
    }

    void OnDrawGizmos()
    {
        
    }
}
