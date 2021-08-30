using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrumCollision : MonoBehaviour
{
    public DrumScript drumParent;
    DrumInteractionVisualization interactionVisualizer;
    public UnityEvent PlayerTouchedDrum;
    public UnityEvent PlayerTouchedDrumVISUALIZE;
    private float collisionCooldown = 0.1f;
    private float lastCollision = 0f;
    
    private float minimumVelocity = 0.5f;
    private float degreeThreshold = 90; // Difference allowed between the normal of the hit and the direction we allow 
    private DrumInteractionVisualization m_interactionVisualization;
    // (UP)
    private readonly Vector3 directionToAllowHitFrom = Vector3.up;

    private BattleDrum m_battleDrum;
    private void Start()
    {
        m_battleDrum = GetComponent<BattleDrum>();
        m_interactionVisualization = GetComponent<DrumInteractionVisualization>();
        PlayerTouchedDrumVISUALIZE.AddListener(test);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("stick"))
        {
            RaycastHit hit;

//            HitDrum();
            
            // Send a ray from the drumstick to the drum
//            if (Physics.Raycast(collision.transform.position, collision.gameObject.transform.forward, out hit))
//            {
//                Vector3 localPoint = hit.transform.InverseTransformPoint(hit.point);
//                Vector3 localDir = localPoint.normalized;
//                float upDot = Vector3.Dot(localDir, transform.up);
//
//                if (upDot<0)
//                {
                    float velocity = collision.gameObject.GetComponent<DrumStickHitDetection>().speed;

                    Debug.Log("Velocity: " + velocity);

                    if (velocity > minimumVelocity)
                    {
                        HitDrum();
                    }
//                }
                
                
//                // If the angle between the normal of the hit and the direction vector up is smaller than some degree
//                if (Vector3.Angle(hit.normal, directionToAllowHitFrom) <= degreeThreshold)
//                {
//                    // Check speed of hit - if its greater than some minimum velocity
////                    Vector3 prevLoc = collision.GetComponent<DrumStickHitDetection>().prevLoc;
////                    float velocity = ((collision.transform.position - prevLoc) / Time.deltaTime).magnitude;
////                    Debug.Log("Velocity: " + velocity);
////                    if (velocity > minimumVelocity)
////                    {
////                    float velocity = collision.attachedRigidbody.velocity.magnitude;
//                    float velocity = collision.gameObject.GetComponent<DrumStickHitDetection>().speed;
//
//                    Debug.Log("Velocity: " + velocity);
//
////                    if (velocity > minimumVelocity)
////                    {
//                        HitDrum();
////                    }
//
////                    }
//                }
//            }
        }
    }

    private void HitDrum()
    {
        lastCollision = Time.time;
        if (m_battleDrum != null && !m_battleDrum.isInteractable) // if it's a battle drum and it's not interactable
        {
            return;
        }
        PlayerTouchedDrum.Invoke();
        m_interactionVisualization.ExitInteractable();
        PlayerTouchedDrumVISUALIZE.Invoke();
    }

    private void test()
    {
        Debug.Log($"TOUCHED A DRUM");
    }




}
