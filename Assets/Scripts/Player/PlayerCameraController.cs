using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public PlayerController playerController;

    public bool isNegativeColor = false;
    private Effects cameraVisualEffects = null;

    private void Start()
    {
        cameraVisualEffects = playerController.playerCamera.GetComponent<Effects>();
        GetComponent<PlayerMovement>().TeleportStart.AddListener(OnTeleportStart);
        GetComponent<PlayerMovement>().TeleportMidway.AddListener(OnTeleportMidway);
        GetComponent<PlayerMovement>().TeleportEnd.AddListener(OnTeleportEnd);
    }

    private void OnTeleportStart()
    {

    }

    private void OnTeleportMidway()
    {
        isNegativeColor = !isNegativeColor;
        cameraVisualEffects.effectType = (isNegativeColor) ? Fx.greyscale : Fx.negative;
        
    }

    private void OnTeleportEnd()
    {

    }
}
