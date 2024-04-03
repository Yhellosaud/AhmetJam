using System;
using UnityEngine;

public class Timer : MonoBehaviour, IDataPersistence
{
    [SerializeField] private int secondsForWaitSession, secondsForPlaySession;
    [SerializeField] private UIContainer UI;
    [SerializeField] private InputListener inputListener;
    private float startTime = 3f;

    public static event Action OnTimeFinished;

    private GameState currentGameState;

    public float StartTime
    {
        get { return startTime; }
        set { startTime = value; }
    }
    public void SaveData(GameData data)
    {
        data.SecondsForWaitSession = secondsForWaitSession;
        data.SecondsForPlaySession = secondsForPlaySession;
    }

    public void LoadData(GameData data)
    {
        secondsForWaitSession = data.SecondsForWaitSession;
        secondsForPlaySession = data.SecondsForPlaySession;
    }
    private void Update()
    {
        if (startTime == 0) return;

        if (startTime > 0)
        {
            startTime -= Time.deltaTime;
        }
        else if (startTime < 0)
        {
            startTime = 0;
            if (currentGameState == GameState.WaitingToStart) inputListener.ManualStart();
            else if (currentGameState == GameState.Play) OnTimeFinished?.Invoke();
        }

        UI.SetTimerText(startTime);
    }
    public void Restart(GameState gameState)
    {
        UI.ToggleTimeText(true);

        if (gameState == GameState.Play) startTime = secondsForPlaySession;
        else if (gameState == GameState.WaitingToStart) startTime = secondsForWaitSession;

        currentGameState = gameState;
    }
    public void Reset()
    {
        startTime = 0;
    }
}
