﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationDrum : MonoBehaviour
{
    public Transform teleportTargetTransform;
    public DrumCollision drumCollision;

    private void Start()
    {
        if (drumCollision != null)
        {
            drumCollision.PlayerTouchedDrum.AddListener(TeleportOnCollision);
            
        }
    }

    private void TeleportOnCollision()
    {
        if (teleportTargetTransform != null)
        {
            PlayerMovement.Instance.TeleportToTarget(teleportTargetTransform);
        }
    }

}