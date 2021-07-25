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
        Debug.Log("recognized trigger with drum ");

        //if (drumParent.interactionController.isInteractable)
        if (collision.CompareTag("stick"))
        {
            RaycastHit hit;
            // Send a ray from the drumstick to the drum
            if (Physics.Raycast(collision.transform.position, collision.transform.forward, out hit))
            {
                // If the angle between the normal of the hit and the direction vector up is smaller than some degree
                if (Vector3.Angle(hit.normal, directionToAllowHitFrom) <= degreeThreshold)
                {
                    // Check speed of hit - if its greater than some minimum velocity
//                    Vector3 prevLoc = collision.GetComponent<DrumStickHitDetection>().prevLoc;
//                    float velocity = ((collision.transform.position - prevLoc) / Time.deltaTime).magnitude;
//                    Debug.Log("Velocity: " + velocity);
//                    if (velocity > minimumVelocity)
//                    {
//                    float velocity = collision.attachedRigidbody.velocity.magnitude;
                    float velocity = collision.gameObject.GetComponent<DrumStickHitDetection>().speed;

                    Debug.Log("Velocity: " + velocity);

                    if (velocity > minimumVelocity)
                    {
                        DrumHit();
                    }

//                    }
                }
            }
        }

        // drum is not interactable yet (cooldown/disabled)
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (drumParent.interactionController.isInteractable)
        if (collision.gameObject.CompareTag("stick"))
        {
            Debug.Log("recognized collision with drum at: " + Time.time);
            RaycastHit hit;
            // Send a ray from the drumstick to the drum
            if (Physics.Raycast(collision.transform.position, collision.transform.forward, out hit))
            {
                // If the angle between the normal of the hit and the direction vector up is smaller than some degree
                if (Vector3.Angle(hit.normal, directionToAllowHitFrom) <= degreeThreshold)
                {
//                    // Check speed of hit - if its greater than some minimum velocity
//                    Vector3 prevLoc = collision.gameObject.GetComponent<DrumStickHitDetection>().prevLoc;
//                    float velocity = ((collision.transform.position - prevLoc) / Time.deltaTime).magnitude;
//                    Debug.Log("Velocity: " + velocity);
//                    if (velocity > minimumVelocity)
//                    {
                    float velocity = collision.relativeVelocity.magnitude;
//                    float velocity = collision.gameObject.GetComponent<DrumStickHitDetection>().speed;


                    Debug.Log("Velocity: " + velocity );
                    if (velocity > minimumVelocity)
                    {
                        DrumHit();
                    }

//                    }
                }
            }
        }
    }


    public void DrumHit()
    {
//        if (lastCollision + collisionCooldown < Time.time)
//        {
            lastCollision = Time.time;
            PlayerTouchedDrum.Invoke();
            PlayerTouchedDrumVISUALIZE.Invoke();
//        }
    }

    private bool IsHitFromBelow()
    {
        return true;
    }

    private void test()
    {
        Debug.Log($"TOUCHED A DRUM");
    }
}