using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrumInteractionVisualization : MonoBehaviour
{

    public GameObject drumTop;
    public GameObject drumBody;

    public bool isInteractable;
    public Color[] colors = new Color[] { Color.white, Color.red, Color.green, Color.blue };
    public Color colorEnabled = Color.white;
    public Color colorDisabled = Color.gray;

    Material drumTopMat;
    public void EnterInteractable()
    {
        if (isInteractable)
        {
            return;
        }
        isInteractable = true;
        drumTopMat.SetColor("_Color", colorEnabled);
    }

    public void ExitInteractable()
    {
        if (!isInteractable)
        {
            return;
        }
        isInteractable = false;
        drumTopMat.SetColor("_Color", colorDisabled);
    }
    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.Instance.OnBeatStart.AddListener(EnterInteractable);
        //AudioManager.Instance.OnBeatEnd.AddListener(ExitInteractable);
        drumTopMat = drumTop.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
