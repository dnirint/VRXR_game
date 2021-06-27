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
    private void Start()
    {
        PlayerTouchedDrumVISUALIZE.AddListener(test);
    }

    void OnTriggerEnter(Collider collision)
    {
        //if (drumParent.interactionController.isInteractable)
        if (lastCollision + collisionCooldown < Time.time)
        {
            lastCollision = Time.time;
            //PlayerTouchedDrum.Invoke();
            PlayerTouchedDrumVISUALIZE.Invoke();
        }
        else
        {
            // drum is not interactable yet (cooldown/disabled)
        }
    }

    private void test()
    {
        Debug.Log($"TOUCHED A DRUM");
    }




}
