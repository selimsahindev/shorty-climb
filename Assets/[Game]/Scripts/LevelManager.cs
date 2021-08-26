using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public bool isGameActive = false;
    [HideInInspector] public Level level;
    [HideInInspector] public PlayerController player;

    [HideInInspector] public UnityEvent startEvent = new UnityEvent();
    [HideInInspector] public UnityEvent onLevelLoaded = new UnityEvent();
    [HideInInspector] public EndGameEvent endGameEvent = new EndGameEvent();

    #region Singleton
    public static LevelManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    private void Start()
    {
        Construct();
    }

    private void Construct()
    {

    }

    public void StartGame()
    {
        isGameActive = true;
        startEvent.Invoke();
    }

    public void Success()
    {
        isGameActive = false;
        endGameEvent.Invoke(true);

        GameManager.instance.LevelUp();
    }

    public void Fail()
    {
        isGameActive = false;
        endGameEvent.Invoke(false);
    }
}

public class EndGameEvent : UnityEvent<bool> { }
