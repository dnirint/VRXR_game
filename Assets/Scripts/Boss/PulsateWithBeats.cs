using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsateWithBeats : MonoBehaviour
{
    public GameObject pulsatingObject;
    public Color pulsatingColorOnBeat;
    public float scaleMultiplierOnStart = 1.1f;
    private Color initialColor;
    private Vector3 initialScale;
    private Material pulsedMat;
    private float pulsateStartTime;
    public float pulsateDuration = 0.1f;
    void Start()
    {
        pulsedMat = pulsatingObject.GetComponent<Renderer>().material;
        initialScale = pulsatingObject.transform.localScale;
        initialColor = pulsedMat.color;
        TimeSignatureController.Instance.ActualBeatEnd.AddListener(Pulsate);
    }

    void Pulsate()
    {
        StartCoroutine(PulsateWorker());
    }

    IEnumerator PulsateWorker()
    {
        pulsateStartTime = Time.time;
        float curPulsateTime = pulsateStartTime;
        float pulsateEndTime = pulsateStartTime + pulsateDuration;
        float curRatio;
        Vector3 pulseStartScale = initialScale * scaleMultiplierOnStart;
        while (curPulsateTime <= pulsateEndTime)
        {
            curRatio = (curPulsateTime - pulsateStartTime) / (pulsateDuration);
            var curColor = Color.Lerp(pulsatingColorOnBeat, initialColor, curRatio);
            var curScale = Vector3.Lerp(pulseStartScale, initialScale, curRatio);
            pulsedMat.color = curColor;
            pulsatingObject.transform.localScale = curScale;
            yield return null;
            curPulsateTime = Time.time;
        }
        pulsedMat.color = initialColor;
        pulsatingObject.transform.localScale = initialScale;
    }
}
