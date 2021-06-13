using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController playerController;
    public static PlayerMovement Instance { get; private set; } = null;

    public bool movementEnabled { get; private set; } = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (playerController != null)
        {
            playerTransform = playerController.player.transform;
        }
    }

    public float teleportSpeed = 2f;
    private Vector3 teleportMoveDirection;
    public Transform playerTransform { get; private set; }
    private bool isTeleporting;
    private static readonly float EPS = 0.5f;
    private static readonly float MOVE_START_EPS = 0.3f;
    private float initialDistanceToTarget;
    private Transform teleportTargetTransform;

    public void TeleportToTarget(Transform target)
    {
        if (movementEnabled && !isTeleporting)
        {
            teleportTargetTransform = target;
            teleportMoveDirection = target.position - playerTransform.position;
            initialDistanceToTarget = teleportMoveDirection.magnitude;
            isTeleporting = true;
        }
    }

    private void Update()
    {
        if (isTeleporting)
        {
            float distToTarget = Vector3.Distance(playerTransform.position, teleportTargetTransform.position);
            if (distToTarget < EPS)
            {
                playerTransform.position = teleportTargetTransform.position;

                isTeleporting = false;
            }
            else
            {
                float pathFraction = (Mathf.Abs(distToTarget - MOVE_START_EPS)) / initialDistanceToTarget;
                float speedModifier = 2 * Mathf.Sin(3 * pathFraction) + 0.3f;
                playerTransform.position += teleportMoveDirection * teleportSpeed * speedModifier * Time.deltaTime;
            }

        }
    }
}
