using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Finish : MonoBehaviour
{
    [SerializeField] private Transform multipliers;

    private void Start()
    {
        foreach (Transform multiplier in multipliers)
        {
            multiplier.localScale = Vector3.zero;
            multiplier.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;

            ParticleSystem particle = Instantiate(GameManager.instance.finishParticles, transform);
            particle.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            particle.transform.localScale = Vector3.one * 1.25f;
            particle.Play();

            ShowMultipliers();

            LevelManager.instance.isFinishReached = true;
            LevelManager.instance.onFinishReached?.Invoke();
        }
    }

    private void ShowMultipliers()
    {
        int i = 0;

        foreach (Transform multiplier in multipliers)
        {
            StartCoroutine(ShowRoutine(multiplier, i * 0.25f));

            Vector3 pos = new Vector3(multiplier.localPosition.x, multiplier.localPosition.y, 2f + (i + 1) * 8f);

            if (i % 2 == 0)
            {
                pos.x = -3f;
            }
            else
            {
                pos.x = -1f;
            }

            multiplier.localPosition = pos;

            i++;
        }
    }

    private IEnumerator ShowRoutine(Transform obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.gameObject.SetActive(true);
        obj.transform.DOScale(1f, 0.35f);
    }
}
