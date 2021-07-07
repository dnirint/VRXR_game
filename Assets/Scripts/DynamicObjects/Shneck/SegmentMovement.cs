using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class SegmentMovement : MonoBehaviour
{


    public PathCreator pathCreator;

    public float snekSpeed;
    public float lastDist;
    public float distOffset;

    private bool isMoving = false;
    void Start()
    {

        lastDist = distOffset;
        transform.position = pathCreator.path.GetPointAtDistance(lastDist);
        transform.rotation = pathCreator.path.GetRotationAtDistance(lastDist);

    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }
    void Update()
    {
        if (isMoving)
        {
            lastDist += Time.deltaTime * snekSpeed;
            transform.position = pathCreator.path.GetPointAtDistance(lastDist);
            transform.rotation = pathCreator.path.GetRotationAtDistance(lastDist);
        }
        
    }
}
