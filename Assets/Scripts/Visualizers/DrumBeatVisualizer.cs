using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBeatVisualizer : MonoBehaviour
{
    public GameObject drumTop;
    public Material drumTopMaterial;
    private bool isInsideBeat = false;
    public Color inBeatColor = Color.white;
    public Color outOfBeatColor = Color.black;
    private bool isTransitioning = false;
    public float transitionTime = 1f;
    public bool shouldVisualize = false;
    void Start()
    {
        drumTopMaterial = drumTop.GetComponent<Renderer>().material;
        if (shouldVisualize)
        {
            AudioManager.Instance.OnBeatStart.AddListener(() => { StartCoroutine(TransitionToActiveColor()); });
            AudioManager.Instance.OnActualBeatEnd.AddListener(TransitionToInactiveColor);
        }

        //AudioManager.Instance.OnActualBeatStart.AddListener(OnEnterBeatVisualize);
        //AudioManager.Instance.OnActualBeatEnd.AddListener(OnExitBeatVisualize);
    }


    void TransitionToInactiveColor()
    {
        if (!isTransitioning)
        {
            isTransitioning = false;
            drumTopMaterial.color = outOfBeatColor;
        }
        
        
    }

    float startTime = 0;
    IEnumerator TransitionToActiveColor()
    {
        float startAfterSeconds = Mathf.Max(0, AudioManager.Instance.timeDifferenceWithBeatDetector - transitionTime);
        Debug.Log($"startAfterSeconds = {startAfterSeconds}");
        yield return new WaitForSeconds(startAfterSeconds);
        if (!isTransitioning)
        {
            Debug.Log($"TRANSITIONING");
            isTransitioning = true;
            startTime = Time.time;
            //PerformTransition();
        }
    }

    private void Update()
    {
        if (isTransitioning)
        {
            float elapsed = Time.time - startTime;
            float ratio = Mathf.Min(elapsed / transitionTime, 1);
            Color lerpedColor = Color.Lerp(outOfBeatColor, inBeatColor, ratio);
            drumTopMaterial.color = lerpedColor;
            if (ratio == 1)
            {
                isTransitioning = false;
            }
        }
    }

    IEnumerator PerformTransition()
    {
        isTransitioning = true;
        float startTime = Time.time;
        float elapsed = 0;
        while (elapsed < transitionTime && isTransitioning)
        {
            elapsed = Time.time - startTime;
            float ratio = Mathf.Min(elapsed / transitionTime, 1);
            Color lerpedColor = Color.Lerp(outOfBeatColor, inBeatColor, ratio);
            drumTopMaterial.color = lerpedColor;
            yield return null;
        }
        isTransitioning = false;
    }



    void OnEnterBeatVisualize()
    {
        if (!isInsideBeat)
        {
            drumTopMaterial.color = inBeatColor;
            isInsideBeat = true;
        }
        
    }

    void OnExitBeatVisualize()
    {
        if (isInsideBeat)
        {
            drumTopMaterial.color = outOfBeatColor;
            isInsideBeat = false;
        }
        
    }

}
