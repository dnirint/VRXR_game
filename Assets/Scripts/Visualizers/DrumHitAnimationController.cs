using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumHitAnimationController : MonoBehaviour
{
    
    
    public GameObject drumTop;

    public Animation drumhitanimation;
    // Start is called before the first frame update
    void Start()
    {
        drumTop.GetComponent<DrumCollision>().PlayerTouchedDrumVISUALIZE.AddListener(AnimateDrumHit);
    }

    void AnimateDrumHit()
    {
        drumhitanimation.Play();
    }
    
    
}
