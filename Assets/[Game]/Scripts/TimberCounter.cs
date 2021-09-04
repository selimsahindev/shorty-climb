using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimberCounter : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private float visibilityTime = 0.5f;

    private bool isActive = false;
    private float timeElapsed = 0f;
    private PlayerController player;
    private Transform parent;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
        canvasGroup = GetComponent<CanvasGroup>();
        parent = transform.parent;

        if (player != null)
        {
            player.onPicked.AddListener(count => {
                UpdateCounter(count);
                ShowCounter();
            });

            player.onPlaced.AddListener(UpdateCounter);
        }

        text.text = "+0";
        canvasGroup.alpha = 0f;
    }

    private void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;
    }

    private void UpdateCounter(int count)
    {
        text.text = "+" + player.timbers.Count.ToString();

        Vector3 pos = player.carryingPosition.position;
        pos += Vector3.up * count * 0.1f;

        parent.position = pos;
    }

    private void ShowCounter()
    {
        transform.DOComplete();
        transform.DOScale(transform.localScale * 1.1f, 0.15f).SetLoops(2, LoopType.Yoyo);

        // Reset timer for visibility
        timeElapsed = 0f;

        // Start new routine if not already visible
        if (!isActive)
        {
            StartCoroutine(FadeInOut());
        }
    }

    private IEnumerator FadeInOut()
    {
        float fadeDuration = 0.25f;

        isActive = true;

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, fadeDuration);

        while (timeElapsed <= visibilityTime)
        {
            yield return new WaitForFixedUpdate();
        }

        canvasGroup.DOFade(0f, fadeDuration);
        isActive = false;
    }
}
