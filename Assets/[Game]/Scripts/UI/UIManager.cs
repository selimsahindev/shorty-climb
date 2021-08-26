using UnityEngine;

public class UIManager : MonoBehaviour
{
    public MainPanel mainPanel;

    #region Singleton
    public static UIManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    private void Start()
    {
        mainPanel.Active(true);

        LevelManager.instance.startEvent.AddListener(OnGameStart);
        LevelManager.instance.endGameEvent.AddListener(OnGameOver);
    }

    public void OnGameStart()
    {
        mainPanel.ActiveSmooth(false);
    }

    public void OnGameOver(bool success)
    {

    }
}