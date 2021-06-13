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
    public float distanceToTarget { get; private set; }
    void Start()
    {
        target = targetGO.transform;
        transform.LookAt(target);
        direction = (target.position - origin).normalized;
        distanceToTarget = Vector3.Distance(transform.position, target.position);
    }

    
    void Update()
    {
        if (trackTarget)
        {
            direction = target.position - origin;
        }
        transform.position += transform.forward * speedFactor * Time.deltaTime;
        distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 0.1)
        {
            TargetHit.Invoke();
        }

    }
}
