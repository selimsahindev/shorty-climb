using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Timber : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Move To Available Carrying Position
            PlayerController player = other.GetComponent<PlayerController>();
            
        }
    }
}
