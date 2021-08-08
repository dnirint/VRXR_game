using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class BeatVisualizer : MonoBehaviour
{
    public float turnOffAfterSeconds = 0.1f;
    public bool visualizeBeats = false;
    public bool visualizeBarStart = true;
    private float turnOffTime = 0f;
    private Material selfMat;
    private bool isOn;

    public static BeatVisualizer Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        selfMat = GetComponent<Renderer>().material;
        TurnOff();
        TimeSignatureController.Instance.Beat.AddListener(TurnOnBeat);
        TimeSignatureController.Instance.CriticalBeat.AddListener(TurnOnBarStart);
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
            isOn = true;
            turnOffTime = Time.time + turnOffAfterSeconds;
        }

    }

    public void TurnOnBarStart()
    {
        if (visualizeBarStart)
        {
            selfMat.color = Color.red;
            isOn = true;
            turnOffTime = Time.time + turnOffAfterSeconds;
        }

    }

    public void TurnOff()
    {   
        selfMat.color = Color.black;
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
