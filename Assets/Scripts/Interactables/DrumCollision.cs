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
    
    private float minimumVelocity = 2;
    private float degreeThreshold = 90; // Difference allowed between the normal of the hit and the direction we allow 

    // (UP)
    private readonly Vector3 directionToAllowHitFrom = Vector3.up;
    
    
    private void Start()
    {
        PlayerTouchedDrumVISUALIZE.AddListener(test);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("stick"))
        {
            
            
            
            RaycastHit hit;

            var direction = collision.transform.position - transform.position;
            if (Physics.Raycast(collision.transform.position, direction, out hit))
            {
                if (hit.collider.gameObject.CompareTag("drumplane"))
                {
                    HitDrum();
                }
            }

            {
                
            }
            
            // Send a ray from the drumstick to the drum
//            if (Physics.Raycast(collision.transform.position, collision.gameObject.transform.forward, out hit))
//            {
//                Vector3 localPoint = hit.transform.InverseTransformPoint(hit.point);
//                Vector3 localDir = localPoint.normalized;
//                float upDot = Vector3.Dot(localDir, transform.up);
//
//                if (upDot<0)
//                {
//                    float velocity = collision.gameObject.GetComponent<DrumStickHitDetection>().speed;
//
//                    Debug.Log("Velocity: " + velocity);
//
//                    if (velocity > minimumVelocity)
//                    {
//                        HitDrum();
//                    }
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
        PlayerTouchedDrum.Invoke();
        PlayerTouchedDrumVISUALIZE.Invoke();
    }

    private void test()
    {
        Debug.Log($"TOUCHED A DRUM");
    }




}
