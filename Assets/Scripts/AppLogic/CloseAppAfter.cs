using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAppAfter : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ExitAppAfterMinutes(3.75f)); // exit after 03:45 minutes
        //StartCoroutine(ExitAppAfterMinutes(0.1f)); // exit after 6 seconds
    }



    IEnumerator ExitAppAfterMinutes(float minutes)
    {
        float seconds = minutes * 60f;
        yield return new WaitForSeconds(seconds);
        UIScript.Instance.ShowEndgameUI(10);
        yield return new WaitForSeconds(10); // exit app after 10 seconds

#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
