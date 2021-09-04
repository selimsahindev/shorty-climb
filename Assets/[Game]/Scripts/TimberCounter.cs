using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimberCounter : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private float visibilityTime = 1.2f;

    private bool isActive = false;
    private int pickedInARow = 0;
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
            player.onPicked.AddListener(() => {
                pickedInARow++;
                UpdateCounter();
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

    private void UpdateCounter()
    {
        text.text = $"+{pickedInARow}";

        Vector3 pos = player.carryingPosition.position;
        pos += Vector3.up * player.timbers.Count * 0.1f;

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
        float fadeDuration = 0.5f;

        isActive = true;

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, fadeDuration);

        while (timeElapsed <= visibilityTime)
        {
            yield return new WaitForFixedUpdate();
        }

        canvasGroup.DOFade(0f, fadeDuration).OnComplete(() => pickedInARow = 0);
        isActive = false;
    }
}
