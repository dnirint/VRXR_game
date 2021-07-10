using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDrum : MonoBehaviour
{

    public int priority = 0; 
    
    void Start()
    {
        GetComponent<DrumScript>().drumCollision.PlayerTouchedDrum.AddListener(() => { BossToPlayerInteractions.Instance.DestroyClosestProjectileOnSameLane(gameObject); });
    }

}
