using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int SecondsForWaitSession, SecondsForPlaySession;

    public int CurrentLevelIndex;

    public List<Level> Levels = new();

    public GameData()
    {

    }
    public GameData(GameSettings gameSettings)
    {
        SecondsForWaitSession = gameSettings.SecondsForWaitSession;
        SecondsForPlaySession = gameSettings.SecondsForPlaySession;

        Levels = new();
        CurrentLevelIndex = 0;
    }
    public GameData(List<Level> levelList)
    {
        Levels = levelList;
    }
}
