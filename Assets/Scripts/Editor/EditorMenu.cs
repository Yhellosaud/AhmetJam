using UnityEditor;
using UnityEngine;
using System.IO;

public class EditorMenu
{
    [MenuItem("My Editor Menu/Reset Saved Data", priority = 500)]
    private static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        File.Delete(Application.persistentDataPath + "/load.game");
    }
}