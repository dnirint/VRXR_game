using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartAnchorPosition : MonoBehaviour
{
    public PlayerController playerController;
    public Transform[] teleportAnchors;
    private Transform playerTransform;
    void Start()
    {
        playerTransform = playerController.player.transform;
        if (teleportAnchors != null && teleportAnchors.Length > 0)
        {
            float minDist = float.PositiveInfinity;
            Vector3 nearestAnchor = Vector3.zero;
            for (int i= 0; i < teleportAnchors.Length; i++)
            {
                var anchorPosition = teleportAnchors[i].position;
                float curDist = Vector3.Distance(playerTransform.position, anchorPosition);
                if (curDist < minDist)
                {
                    minDist = curDist;
                    nearestAnchor = anchorPosition;
                }
            }

            playerTransform.position = nearestAnchor;
            Debug.Log($"Snapped player to {nearestAnchor}");
        }
    }
}
