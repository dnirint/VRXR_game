using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumHitVisualizer : MonoBehaviour
{
    public GameObject outerRing;
    public GameObject innerRing;
    public GameObject drumTop;
    public GameObject drumTopParent;


    bool isReacting = false;
    float reactStartTime = 0;
    private const float reactDurationInSeconds = 0.15f;
    public float reactBlipFactor = 0.5f;
    void Start()
    {
        //startingScale = transform.localScale;
        //startingScale = outerRing.transform.localScale;
        startingScale = drumTopParent.transform.localScale;
        drumTop.GetComponent<DrumCollision>().PlayerTouchedDrumVISUALIZE.AddListener(PerformReact);
    }
    private Vector3 startingScale;
    void ResetScale()
    {
        //transform.localScale = startingScale;
        //outerRing.transform.localScale = startingScale;
        drumTopParent.transform.localScale = startingScale;
    }

    void Update()
    {
        if (isReacting)
        {
            if (reactStartTime + reactDurationInSeconds < Time.time)
            {
                isReacting = false;
                ResetScale();
            }
            else
            {
                float factor = (Time.time - reactStartTime) / reactDurationInSeconds;
                //transform.localScale = startingScale + startingScale * -Mathf.Sin(Mathf.PI*factor)* reactBlipFactor;
                //outerRing.transform.localScale = startingScale + startingScale * -Mathf.Sin(Mathf.PI*factor)* reactBlipFactor;
                drumTopParent.transform.localScale = startingScale + startingScale * -Mathf.Sin(Mathf.PI*factor)* reactBlipFactor;
            }
        }
    }

    public void PerformReact()
    {
        if (isReacting)
        {
            return;
        }
        reactStartTime = Time.time;
        isReacting = true;
    }
}
