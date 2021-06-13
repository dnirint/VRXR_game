using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumInteractionController : MonoBehaviour
{
    public bool isEnabled { get; private set; } = true;

    public bool isInteractable { get; private set; } = false;

    public float interactionCooldown { get; private set; } = 0.1f;

    private float lastInteractionTime = 0f;

    public void OnEnterCooldown()
    {
        lastInteractionTime = Time.time;
        isInteractable = false;
    }

    void Update()
    {
        if (isEnabled && !isInteractable && lastInteractionTime + interactionCooldown < Time.time)
        {
            isInteractable = true;
        }
    }

    public void SetEnabled(bool val)
    {
        isInteractable = val && isInteractable;
        isEnabled = val;
    }
}
