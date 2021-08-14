using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInversion : MonoBehaviour
{
    private bool inverted = false;
    public GameObject inversionField;
    void Start()
    {
        PlayerMovement.Instance.TeleportMidway.AddListener(InverseColor);    
        inversionField.SetActive(inverted);
    }

    void InverseColor()
    {
        inverted = !inverted;
        inversionField.SetActive(inverted);
    }



}
