using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeEffectWall : MonoBehaviour
{

   private bool isNegativeColor = false;
   
   public Effects cameraVisualEffects = null;
   public PlayerController playerController;
   private void Start()
   {
      if (cameraVisualEffects==null)
      {
         cameraVisualEffects = playerController.playerCamera.GetComponent<Effects>();
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         isNegativeColor = !isNegativeColor;
         cameraVisualEffects.effectType = (isNegativeColor) ? Fx.greyscale : Fx.negative;
      }
   }
}
