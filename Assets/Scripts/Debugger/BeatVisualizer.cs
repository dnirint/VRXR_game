using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;


public class BeatVisualizer : MonoBehaviour
{
    public float turnOffAfterSeconds = 0.1f;
    public bool visualizeBeats = false;
    public bool visualizeBarStart = true;
    private float turnOffTime = 0f;
    private Material selfMat;
    private bool isOn;
    public static BeatVisualizer Instance = null;

    private float beatTime;
    private Vector3 initialScale;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    private void Start()
    {
        initialScale = transform.localScale;
        selfMat = GetComponent<Renderer>().material;
        TurnOff();
        beatTime = TimeSignatureController.Instance.beatDurationForPlayer;
        TimeSignatureController.Instance.BeatEnd.AddListener(TurnOnBeat);
        TimeSignatureController.Instance.CriticalBeatEnd.AddListener(TurnOnBarStart);
    }

    void Update()
    {
        if (isOn && Time.time > turnOffTime)
        {
            TurnOff();
        }

    }
    public void TurnOnBeat()
    {
        if (visualizeBeats)
        {
            selfMat.color = Color.white;
            transform.localScale = initialScale * 0.9f;
            isOn = true;
            turnOffTime = Time.time + TimeSignatureController.Instance.timeBetweenBeats * 0.7f; ;
            
        }

    }

    public void TurnOnBarStart()
    {
        if (visualizeBarStart)
        {
            selfMat.color = Color.red;
            transform.localScale = initialScale * 0.9f;
            isOn = true;
            turnOffTime = Time.time + TimeSignatureController.Instance.timeBetweenBeats * 0.7f; ;
        }

    }

    public void TurnOff()
    {   
        selfMat.color = Color.black;
        transform.localScale = initialScale;
        isOn = false;
    }
}


[CustomEditor(typeof(BeatVisualizer))]
public class BeatVisualizerEditorControls : Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Label("Visualizer Controlls");
        if (GUILayout.Button("Visualize Beat"))
        {
            BeatVisualizer.Instance.TurnOnBeat();
        }

    }
}
