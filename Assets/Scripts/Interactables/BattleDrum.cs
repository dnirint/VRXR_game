using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDrum : MonoBehaviour
{

    public int priority = 0;

    public bool isInteractable = false;

    private DrumInteractionVisualization m_div;
    public void SetInteractable(bool targeted)
    {
        isInteractable = targeted;
        if (isInteractable)
        {
            m_div.EnterInteractable();
        }
        else
        {
            m_div.ExitInteractable();
        }
    }
    void Start()
    {
        m_div = GetComponent<DrumInteractionVisualization>();
        GetComponent<DrumScript>().drumCollision.PlayerTouchedDrum.AddListener(OnDrumHit);
    }

    void OnDrumHit()
    {
        if (isInteractable)
        {
            BossToPlayerInteractions.Instance.DestroyClosestProjectileOnSameLane(gameObject);
        }
        
    }

}
