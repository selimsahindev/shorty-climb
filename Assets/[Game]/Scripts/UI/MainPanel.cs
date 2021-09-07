using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainPanel : Panel
{
    public RectTransform startButton;

    private void Start()
    {
        startButton.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void OnPressStart()
    {
        LevelManager.instance.StartGame();
    }
}