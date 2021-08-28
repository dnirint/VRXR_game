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
    public UnityEvent OnTimeout;
    
    
    
    public float distanceToTarget { get; private set; }

    void Start()
    {
        Debug.Log($"New projectile with {totalFlightTime}");
        //        startingPos = transform.position;
        target = targetGO.transform;
        targetPos = target.position;
        transform.LookAt(target);
        direction = (targetPos - origin).normalized;
        //TargetHit.AddListener(() => { Destroy(gameObject); });
        OnTimeout.AddListener(OnTimeoutReached);

        StartCoroutine(MoveToTargetInTime(TargetHit, OnTimeout));
    }

    void OnTimeoutReached()
    {
        Debug.Log($"TIMEOUT!!");

        Destroy(gameObject);
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

    //[SerializeField] private float timeToTarget = 10f;
    [SerializeField] public float totalFlightTime = 1f;


    //    private Vector3 startingPos;
    private Vector3 targetPos;
    //    private void MoveToTargetInSetTime()
    //    {
    //        currentTime += Time.deltaTime / timeToTarget;
    //        transform.position = Vector3.Lerp(startingPos, targetPos, currentTime);
    //    }

    public float timeToTarget;
    IEnumerator MoveToTargetInTime(UnityEvent onTargetHit = null, UnityEvent onTimeout = null)
    {
        float startTime = Time.time;
        float elapsed = 0;
        Vector3 startingPos = transform.position;
        while (elapsed < totalFlightTime)
        {
            elapsed = Time.time - startTime;
            float ratio = Mathf.Min(elapsed / totalFlightTime, 1);

            transform.position = Vector3.Lerp(startingPos, targetPos, ratio);
            distanceToTarget = Vector3.Distance(transform.position, target.position);
            timeToTarget = totalFlightTime - elapsed;
            if (distanceToTarget < 0.3f)
            {
                TargetHit.Invoke();
            }

            yield return null;
        }

        if (onTimeout != null)
        {
            onTimeout.Invoke();
        }
    }

    #endregion

    void OnDrumHit()
    {
        TargetHit.Invoke();
        MapLogic.Instance.ProjectileHitDrum.Invoke();
    }

    private void OnDestroy()
    {
        OnTimeout.RemoveAllListeners();
        TargetHit.RemoveAllListeners();
    }
}