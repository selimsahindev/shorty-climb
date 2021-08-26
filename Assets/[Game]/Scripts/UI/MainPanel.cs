using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainPanel : Panel
{
    public Text levelText;
    public Text continueText;

    private void Start()
    {
        levelText.text = "LEVEL " + GameManager.instance.level;

        continueText.rectTransform.DOScale(1.2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void OnPressStart()
    {
        LevelManager.instance.StartGame();
    }
}