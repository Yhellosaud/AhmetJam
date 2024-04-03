using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings", order = 0)]
public class GameSettings : ScriptableObject
{
    [SerializeField] private int secondsForWaitSession = 5;
    [SerializeField] private int secondsForPlaySession = 10;

    public int SecondsForWaitSession { get { return secondsForWaitSession; } }
    public int SecondsForPlaySession { get { return secondsForPlaySession; } }

}
