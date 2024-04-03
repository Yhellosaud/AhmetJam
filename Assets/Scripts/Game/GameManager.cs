
using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;


public enum GameState
{
    WaitingToStart,
    Play,
    Finish
}
public class GameManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Timer timer;
    [SerializeField] private InputListener inputListener;
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private UIContainer UI;
    public static event Action OnGameStart, OnGameFail;

    private CancellationTokenSource source = new();

    public GameState GameState = GameState.WaitingToStart;
    public List<Level> Levels = new();
    public int CurrentLevelIndex;
    private bool isReadyToStart;
    public void SaveData(GameData data)
    {
    }

    public void LoadData(GameData data)
    {
        Levels = data.Levels;
        Debug.Log("Count 2: " + Levels.Count);

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                Debug.Log("Fetch: " + Levels[0].topGrid[x, y]);
            }
        }
        CurrentLevelIndex = data.CurrentLevelIndex;
    }

    private void OnEnable()
    {
        InputListener.OnTapToStart += StartGame;
        Timer.OnTimeFinished += Fail;
        ColorTweener.OnTransitionComplete += () => { isReadyToStart = true; };
    }
    private void OnDisable()
    {
        ColorTweener.OnTransitionComplete -= () => { isReadyToStart = true; };
        InputListener.OnTapToStart -= StartGame;
        Timer.OnTimeFinished -= Fail;
    }

    public GameState ChangeGameState(GameState gameState)
    {
        if (gameState == GameState) return gameState;

        GameState = gameState;

        return gameState;
    }

    public async void StartGame(bool type)
    {
        await UniTask.WaitUntil(() => IsTransitionComplete());

        if (source.IsCancellationRequested) return;

        OnGameStart?.Invoke();

        timer.Restart(ChangeGameState(GameState.Play));

        if (CurrentLevelIndex >= Levels.Count) CurrentLevelIndex = 0;
        UI.SetLevelText(CurrentLevelIndex + 1);


        levelGenerator.Generate(Levels[CurrentLevelIndex]);

        isReadyToStart = false;

    }
    public void Pass()
    {
        timer.Reset();
        CurrentLevelIndex++;
    }
    public void Fail()
    {
        ChangeGameState(GameState.Finish);
    }
    public void Restart()
    {
        inputListener.Reset();
        timer.Restart(ChangeGameState(GameState.WaitingToStart));
    }
    private bool IsTransitionComplete()
    {
        return isReadyToStart;
    }
}
