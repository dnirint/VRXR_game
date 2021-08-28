using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrumInteractionVisualization : MonoBehaviour
{

    public GameObject drumTop;
    public float interactablePeriod;
    public bool isInteractable;
    public Color colorEnabled = Color.white;
    private Color colorDisabled = Color.gray;
    private BattleDrum m_battleDrum;
    Material drumTopMat;
    public void EnterInteractable()
    {
        if (!m_battleDrum.isTargeted || isInteractable)
        {
            return;
        }
        interactablePeriod = TimeSignatureController.Instance.preBeatTime;
        Debug.Log($"Enter interactable");
        isInteractable = true;
        drumTopMat.color = colorEnabled;
        StartCoroutine(ExitInteractableAfterSeconds(interactablePeriod));
    }

    IEnumerator ExitInteractableAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ExitInteractable();
        yield return null;
    }

    public void ExitInteractable()
    {
        if (!isInteractable)
        {
            return;
        }
        isInteractable = false;
        drumTopMat.color = colorDisabled;
    }
    // Start is called before the first frame update
    void Start()
    {
        //TimeSignatureController.Instance.ActualCriticalBeatStart.AddListener(EnterInteractable);
        m_battleDrum = GetComponent<BattleDrum>();
        drumTopMat = drumTop.GetComponent<Renderer>().material;
        colorDisabled = drumTopMat.color;
}

    // Update is called once per frame
    void Update()
    {

    }
}
