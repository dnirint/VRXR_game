using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumScript : MonoBehaviour
{
    public GameObject drumTop;
    public GameObject drumBody;
    public DrumInteractionController interactionController;
    public DrumCollision drumCollision;

    // Start is called before the first frame update
    void Start()
    {
        // connect the drum collision detection to the interaction cooldown system
        if (drumTop != null && interactionController != null)
        {
            drumTop.GetComponent<DrumCollision>().PlayerTouchedDrum.AddListener(interactionController.OnEnterCooldown);
        }
        
    }

}
