using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DebuggerScript : MonoBehaviour
{
    [SerializeField]
    public TeleportationDrum leftPlatformRightTeleport;
    [SerializeField]
    public TeleportationDrum RightPlatformLeftTeleport;

    public static DebuggerScript Instance { get; private set; } = null;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[CustomEditor(typeof(DebuggerScript))]
public class DebuggerScriptEditorControls : Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Label("Left Platform");
        if (GUILayout.Button("Right teleport"))
        {
            DebuggerScript.Instance.leftPlatformRightTeleport.TeleportOnCollision();
        }
        GUILayout.Label("Right Platform");
        if (GUILayout.Button("Left teleport"))
        {
            DebuggerScript.Instance.RightPlatformLeftTeleport.TeleportOnCollision();
        }

    }
}

