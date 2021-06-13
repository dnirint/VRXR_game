using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrumCollision : MonoBehaviour
{
    public DrumScript drumParent;
    DrumInteractionVisualization interactionVisualizer;
    public UnityEvent PlayerTouchedDrum;


    void OnTriggerEnter(Collider collision)
    {
        if (drumParent.interactionController.isInteractable)
        {
            PlayerTouchedDrum.Invoke();
        }
        else
        {
            // drum is not interactable yet (cooldown/disabled)
        }
    }
}
