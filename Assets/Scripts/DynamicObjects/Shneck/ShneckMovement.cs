using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ShneckMovement : MonoBehaviour
{

    public GameObject head;
    public List<GameObject> bodySegments;
    public GameObject bodyParent;
    public GameObject bodySegmentPrefab;
    public GameObject tail;
    public PathCreator pathCreator;
    public int bodySegmentCount = 5;
    public float bodySegmentDistance = 4f;
    public float bodyDistanceFromHead = 1f;
    public float snekSpeedFactor = 1f;
    public float lapTimeInSeconds = 5f;
    float totalPathLength;
    public float snekSpeed;
    public List<SegmentMovement> snekSegments = new List<SegmentMovement>();
    void Start()
    {

        totalPathLength = pathCreator.path.length;
        snekSpeed = totalPathLength / lapTimeInSeconds;
        Debug.Log($"------path len = {totalPathLength}, speed = {snekSpeed} units/sec");

        // build the snek
        BuildSnek();


    }

    void BuildSnek()
    {
        var headMovement = head.GetComponent<SegmentMovement>();
        var tailMovement = tail.GetComponent <SegmentMovement>();
        headMovement.snekSpeed = snekSpeed;
        headMovement.distOffset = 0;
        headMovement.pathCreator = pathCreator;
        snekSegments.Add(headMovement);
        float totalDistOffset = 0;
        var bodyPartDistanceWithHeadOffset = bodyDistanceFromHead + bodySegmentDistance;
        for (int i=1; i<=bodySegmentCount; i++) // start from 1 so that the first segment won't collide with the head
        {
            var segGo = Instantiate(bodySegmentPrefab, bodyParent.transform);
            var segMove = segGo.GetComponent<SegmentMovement>();
            segMove.snekSpeed = snekSpeed;
            totalDistOffset = -i * bodySegmentDistance - bodyDistanceFromHead;
            segMove.distOffset = totalDistOffset;
            segMove.pathCreator = pathCreator;
            snekSegments.Add(segMove);
            bodySegments.Add(segGo);
        }
        totalDistOffset -= bodySegmentDistance;
        tailMovement.distOffset = totalDistOffset ;
        Debug.Log($"--> {totalDistOffset}");
        tailMovement.snekSpeed = snekSpeed;
        snekSegments.Add(tailMovement);
        StartSnek();

    }

    public void StartSnek()
    {
        foreach (var segment in snekSegments)
        {
            segment.LocalizeAndStartMoving();
        }
    }

    public void StopSnek()
    {
        foreach (var segment in snekSegments)
        {
            segment.StopMoving();
        }
    }

    void Update()
    {
    }
}
