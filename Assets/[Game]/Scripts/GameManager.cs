using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public int levelCount;
    public int level = -1;

    [HideInInspector] public bool isGamePaused = false;

    [HideInInspector] public UnityEvent onGamePaused = new UnityEvent();
    [HideInInspector] public UnityEvent onGameContinue = new UnityEvent();

    #region Singleton
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            //GetDependencies();
        }
        else
        {
            DestroyImmediate(this);
        }
    }
    #endregion

    //private void GetDependencies()
    //{
    //    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || level == -1)
    //    {
    //        level = DataManager.instance.level;
    //    }
    //    money = DataManager.instance.money;
    //}

    public void PauseGame()
    {
        isGamePaused = true;
        onGamePaused.Invoke();
    }

    public void ContinueGame()
    {
        isGamePaused = false;
        onGameContinue.Invoke();
    }

    #region DataOperations
    public void LevelUp()
    {
        if (++level > levelCount)
            level = 1;
        //DataManager.instance.SetLevel(level);
    }
    #endregion

    #region SceneOperations
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OpenScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    #endregion
}