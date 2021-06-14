using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public Vector3 origin;
    public GameObject targetGO;
    public float speedFactor = 10f;
    public bool trackTarget = false; // if the projectile should follow the target or just go to the original position
    public UnityEvent TargetHit;
    private Transform target;
    private Vector3 direction;

    void Start()
    {
//        startingPos = transform.position;
        target = targetGO.transform;
        targetPos = target.position;
        transform.LookAt(target);
        direction = (targetPos - origin).normalized;
        StartCoroutine(MoveToTargetInTime());
    }

//    
//    void Update()
//    {
//        if (trackTarget)
//        {
//            direction = target.position - origin;
//        }
////        transform.position += transform.forward * speedFactor * Time.deltaTime;
////        MoveToTargetInSetTime();
//        if (Vector3.Distance(transform.position, target.position) < 0.1)
//        {
//            TargetHit.Invoke();
//            Destroy(gameObject);
//        }
//
//    }


    #region ProjectileMovementOverTime

    [SerializeField] private float timeToTarget = 10f;

    private float currentTime = 0;

//    private Vector3 startingPos;
    private Vector3 targetPos;
//    private void MoveToTargetInSetTime()
//    {
//        currentTime += Time.deltaTime / timeToTarget;
//        transform.position = Vector3.Lerp(startingPos, targetPos, currentTime);
//    }

    IEnumerator MoveToTargetInTime()
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime / timeToTarget;
            transform.position = Vector3.Lerp(startingPos, targetPos, elapsedTime);
            yield return null;
        }

        if (Vector3.Distance(transform.position, target.position) < 0.1)
        {
            TargetHit.Invoke();
            Destroy(gameObject);
        }
    }

    #endregion
}