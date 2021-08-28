using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDrum : MonoBehaviour
{

    public int priority = 0;

    public bool isTargeted = false;

    private DrumInteractionVisualization m_div;
    public void SetTargeted(bool targeted)
    {
        isTargeted = targeted;
        if (isTargeted)
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

        BossToPlayerInteractions.Instance.DestroyClosestProjectileOnSameLane(gameObject);
    }

}
