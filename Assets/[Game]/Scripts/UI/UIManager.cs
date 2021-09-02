using UnityEngine;

public class UIManager : MonoBehaviour
{
    public MainPanel mainPanel;
    public GamePanel gamePanel;

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
        gamePanel.Active(false);

        LevelManager.instance.startEvent.AddListener(OnGameStart);
        LevelManager.instance.endGameEvent.AddListener(OnGameOver);
    }

    public void OnGameStart()
    {
        mainPanel.ActiveSmooth(false);
        gamePanel.ActiveSmooth(true);
    }

    public void OnGameOver(bool success)
    {
        
    }
}