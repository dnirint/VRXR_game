﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDrum : MonoBehaviour
{

    public int priority = 0;
    public GameObject drumPrefab;
    
    
    
    void Start()
    {
        GetComponent<DrumScript>().drumCollision.PlayerTouchedDrum.AddListener(OnDrumHit);
    }

    void OnDrumHit()
    {

        BossToPlayerInteractions.Instance.DestroyClosestProjectileOnSameLane(gameObject);
    }

}
