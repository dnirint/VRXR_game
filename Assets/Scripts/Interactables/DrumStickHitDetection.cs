using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumStickHitDetection : MonoBehaviour
{
    private Vector3 prevLoc;

    public float speed;

    private void Start()
    {
        prevLoc = transform.position;
    }

    private void Update()
    {
        speed = Vector3.Distance(transform.position, prevLoc) / Time.deltaTime;
        prevLoc = transform.position;
    }
}