using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiplier : MonoBehaviour
{
    [SerializeField] private int value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ParticleSystem particle = Instantiate(GameManager.instance.finishParticles, transform);
            particle.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            particle.transform.localScale = Vector3.one * (value == 15 ? 1f : 0.5f);
            particle.Play();

            LevelManager.instance.multiplier = this;
        }
    }
}
