using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour
{
    Dictionary<string, string> debugLogs = new Dictionary<string, string>();

    public Text display;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }


    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            string[] splitSpring = logString.Split(char.Parse(":"));
            string debugKey = splitSpring[0];
            string debugValue = splitSpring.Length > 1 ? splitSpring[1] : "";


            if (debugLogs.ContainsKey(debugKey))
            {
                debugLogs[debugKey] = debugValue;
            }
            else
            {
                debugLogs.Add(debugKey, debugValue);
            }
        }

        string displayText = "";
        foreach (KeyValuePair<string, string> log in debugLogs)
        {
            if (log.Value == "")
            {
                displayText += log.Key + "\n";
            }
            else
            {
                displayText += log.Key + ": " + log.Value + "\n";
            }
        }

        display.text = displayText;
    }
}