using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapLogic : MonoBehaviour
{
    
    
    public static MapLogic Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public UnityEvent ProjectileHitDrum;
    
    
    




}
