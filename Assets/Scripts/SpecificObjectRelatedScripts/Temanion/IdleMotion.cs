using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleMotion : MonoBehaviour
{
    public bool isBossComponent = true;
    [Header("Idle float")]
    public Vector3 floatDirection = Vector3.up;
    public float floatSpeed = 1f;
    public float floatRangeFactor = 1f;
    [Header("Idle rotate around self")]
    public float selfRotationSpeed = 5f;
    [Header("Idle orbit around another object")]
    public Transform orbitCenter = null;
    public Vector3 orbitAxis = Vector3.up;
    public float orbitSpeed = 5f;
    [Header("Idle scale change (breathing)")]
    public float scaleFactorX = 0.1f;
    public float scaleFactorY = 0.1f;
    public float scaleFactorZ = 0.1f;
    public float scaleSpeed = 1f;
    [Header("ON-OFF switches")]
    public bool shouldFloat = false;
    public bool shouldSelfRotate = false;
    public bool shouldOrbit = false;
    public bool shouldScale = false;





    private Vector3 startingScale;

    private void LateUpdate()
    {
        Debug.DrawRay(transform.position, transform.up, color:Color.white);
        Debug.DrawRay(transform.position, transform.forward, color:Color.green);
        if (shouldFloat)
        {
            DoFloat();
        }
        if (shouldSelfRotate)
        {
            DoSelfRotate();
        }
        if (shouldOrbit)
        {
            DoOrbit();
        }
        if (shouldScale)
        {
            DoScale();
        }
    }

    private void DoScale()
    {
        transform.localScale = startingScale + new Vector3(scaleFactorX, scaleFactorY, scaleFactorZ) * (0.5f+0.5f*Mathf.Sin(scaleSpeed * Time.time));
    }

    private void DoOrbit()
    {
        var orbitCenterTransform = (isBossComponent) ? BossController.Instance.boss.transform : orbitCenter;
        var orbitCenterVector = orbitCenterTransform.forward;
        var directionFromCenter = transform.position - orbitCenter.position;
        orbitAxis = Vector3.Cross(orbitCenterVector.normalized, directionFromCenter.normalized).normalized;
        Debug.DrawLine(transform.position, orbitCenterTransform.position, color: Color.yellow);
        Debug.DrawRay(orbitCenterTransform.position, orbitCenterVector, color: Color.cyan);
        transform.RotateAround(orbitCenter.position, orbitCenterVector, orbitSpeed * Time.deltaTime);
    }

    private void DoSelfRotate()
    {
        transform.RotateAround(transform.position, transform.forward, selfRotationSpeed * Time.deltaTime);
    }

    private Vector3 startPosition;
    private void DoFloat()
    {
        transform.position = startPosition + floatDirection * (0.5f*Mathf.Sin(floatSpeed * Time.time)) * floatRangeFactor;
    }

    private void Start()
    {
        startPosition = transform.position;
        startingScale = transform.localScale;
    }

    public void SetAllMotion(bool val)
    {
        shouldFloat = val;
        shouldSelfRotate = val;
        shouldOrbit = val;
        shouldScale = val;
    }


}
