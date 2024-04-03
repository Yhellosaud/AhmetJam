using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Level
{
    public int number;
    public int bottomGridRowCount;
    public int bottomGridColumnCount;
    public int topGridRowCount;
    public int topGridColumnCount;
    public List<ColorType> colorList;
    public bool[,] topGrid;

    public int[,] bottomGridColors;


    public Level(bool[,] topGrid, int[,] bottomGridColors, int topGridRowCount, int topGridColumnCount, int bottomGridRowCount, int bottomGridColumnCount, List<ColorType> colorList)
    {
        this.topGrid = topGrid;
        this.bottomGridColors = bottomGridColors;
        this.colorList = colorList;

        this.bottomGridRowCount = bottomGridRowCount;
        this.bottomGridColumnCount = bottomGridColumnCount;
        this.topGridRowCount = topGridRowCount;
        this.topGridColumnCount = topGridColumnCount;
    }
}
public enum ColorType
{
    Empty,
    Red,
    Blue,
    Yellow,
    White
}

public class LevelEditorWindow : EditorWindow
{
    private int currentItemIndex = 0;
    public List<Level> Levels = new List<Level>();

    private Vector2 scrollPosition;

    private ColorType selectedColor = ColorType.Empty;

    private FileDataHandler fileDataHandler;

    private const string BottomRowsKey = "BottomRows";
    private const string BottomColumnsKey = "BottomColumns";
    private const string TopRowsKey = "TopRows";
    private const string TopColumnsKey = "TopColumns";

    private Level currentItem;
    private Level thisItem = new Level(new bool[2, 2], new int[2, 2] { { 0, 0 }, { 0, 0 } }, 2, 2, 2, 2, new List<ColorType> { ColorType.Empty });



    [MenuItem("My Editor Menu/Configure Levels")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Bus Jam Grid Designer");
    }

    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginHorizontal();

        GUILayout.Label("Levels", EditorStyles.boldLabel);
        DrawItemList();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save", GUILayout.Width(100)))
        {
            fileDataHandler.Save(new GameData(Levels));
        }
        if (GUILayout.Button("Delete all", GUILayout.Width(100)))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            EditorPrefs.DeleteAll();

            // File.Delete(Application.persistentDataPath + "/load.game");
            fileDataHandler.Save(new GameData(new List<Level>()));
            Levels.Clear();
            GameData data = fileDataHandler.Load();

            Levels = data.Levels;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        GUILayout.Label("Level " + (currentItemIndex + 1), EditorStyles.boldLabel);
        GUILayout.BeginVertical();
        GUI.enabled = currentItemIndex > 0;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Previous Level"))
        {
            currentItemIndex--;
        }
        GUI.enabled = currentItemIndex < Levels.Count - 1;
        if (GUILayout.Button("Next Level"))
        {
            currentItemIndex++;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUI.enabled = true;

        if (Levels.Count > 0 && currentItemIndex >= 0 && currentItemIndex < Levels.Count)
        {
            currentItem = Levels[currentItemIndex];
            EditorGUI.BeginChangeCheck();
            DrawColorOrderList();
            if (EditorGUI.EndChangeCheck())
            {
            }

            DrawGrid(ref currentItem.topGrid, TopRowsKey + currentItemIndex, TopColumnsKey + currentItemIndex);
            // GUILayout.FlexibleSpace();

            DrawBottomGrid(ref Levels[currentItemIndex].bottomGridColors, BottomRowsKey + currentItemIndex, BottomColumnsKey + currentItemIndex);
        }
        GUILayout.Space(20);
        GUILayout.EndScrollView();


    }

    void DrawBottomGrid(ref int[,] grid, string rowsKey, string columnsKey)
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Passanger Start Grid", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Rows:");
        int rows = EditorGUILayout.IntField(EditorPrefs.GetInt(rowsKey, 1), GUILayout.Width(50), GUILayout.Height(50));
        GUILayout.Label("Columns:");
        int columns = EditorGUILayout.IntField(EditorPrefs.GetInt(columnsKey, 1), GUILayout.Width(50), GUILayout.Height(50));
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
        grid = ResizeGrid(grid, rows, columns);

        for (int y = 0; y < rows; y++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            for (int x = 0; x < columns; x++)
            {
                EditorGUI.BeginChangeCheck();
                ColorType colorType = (ColorType)EditorGUILayout.EnumPopup((ColorType)grid[x, y], GUILayout.Width(80), GUILayout.Height(20));
                grid[x, y] = (int)colorType;
                Levels[currentItemIndex].bottomGridColors[x, y] = grid[x, y];
                Levels[currentItemIndex].bottomGridRowCount = rows;
                Levels[currentItemIndex].bottomGridColumnCount = columns;

                if (EditorGUI.EndChangeCheck())
                {
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        EditorPrefs.SetInt(rowsKey, rows);
        EditorPrefs.SetInt(columnsKey, columns);
    }

    void DrawItemList()
    {
        // Draw item list
        GUILayout.BeginVertical();
        // Add button
        if (GUILayout.Button("Add new level"))
        {
            int colorType = Enum.GetNames(typeof(ColorType)).Length - 1;
            Levels.Add(thisItem);
            UpdateItemNumbers();
        }
        GUILayout.Label("Level List", EditorStyles.boldLabel);

        for (int i = 0; i < Levels.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select"))
            {
                currentItemIndex = i;
                GUILayout.EndHorizontal();
                break;
            }
            // Remove button
            if (GUILayout.Button("Remove"))
            {
                Levels.RemoveAt(i);
                UpdateItemNumbers();
                GUILayout.EndHorizontal();
                break;
            }

            // Up button
            if (i > 0 && GUILayout.Button("▲", GUILayout.Width(20)))
            {
                var temp = Levels[i - 1];
                Levels[i - 1] = Levels[i];
                Levels[i] = temp;
                SwapItemContents(Levels[i - 1], Levels[i]);
            }

            // Down button
            if (i < Levels.Count - 1 && GUILayout.Button("▼", GUILayout.Width(20)))
            {
                var temp = Levels[i + 1];
                Levels[i + 1] = Levels[i];
                Levels[i] = temp;
                SwapItemContents(Levels[i], Levels[i + 1]);
            }

            // Draw number for each item
            GUILayout.Label((i + 1).ToString(), GUILayout.Width(20));

            GUILayout.EndHorizontal();
        }



        GUILayout.EndVertical();
    }

    void UpdateItemNumbers()
    {
        for (int i = 0; i < Levels.Count; i++)
            Levels[i].number = i + 1;
    }
    void SwapItemContents(Level item1, Level item2)
    {
        // Color
        List<ColorType> tempColorList = item1.colorList;
        item1.colorList = item2.colorList;
        item2.colorList = tempColorList;

        // TopGrid
        bool[,] tempTopGrid = item1.topGrid;
        item1.topGrid = item2.topGrid;
        item2.topGrid = tempTopGrid;

        // BottomGrid
        int[,] tempBottomGrid = item1.bottomGridColors;
        item1.bottomGridColors = item2.bottomGridColors;
        item2.bottomGridColors = tempBottomGrid;
    }

    void DrawGrid(ref bool[,] grid, string rowsKey, string columnsKey)
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Passanger Queue Grid", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Rows:");
        int rows = EditorGUILayout.IntField(EditorPrefs.GetInt(rowsKey, 1), GUILayout.Width(50), GUILayout.Height(50));
        GUILayout.Label("Columns:");
        int columns = EditorGUILayout.IntField(EditorPrefs.GetInt(columnsKey, 1), GUILayout.Width(50), GUILayout.Height(50));
        GUILayout.EndHorizontal();

        grid = ResizeGrid(grid, rows, columns);
        GUILayout.BeginVertical();
        for (int y = 0; y < rows; y++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            for (int x = 0; x < columns; x++)
            {
                EditorGUI.BeginChangeCheck();
                grid[x, y] = EditorGUILayout.Toggle(true, GUILayout.Width(20), GUILayout.Height(20));
                if (EditorGUI.EndChangeCheck())
                {
                    // Handle change in grid cell
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        // GUILayout.EndHorizontal();
        EditorPrefs.SetInt(rowsKey, rows);
        EditorPrefs.SetInt(columnsKey, columns);
    }

    bool[,] ResizeGrid(bool[,] originalGrid, int newRows, int newColumns)
    {
        if (originalGrid == null)
            return new bool[newColumns, newRows];

        int originalRows = originalGrid.GetLength(1);
        int originalColumns = originalGrid.GetLength(0);
        bool[,] resizedGrid = new bool[newColumns, newRows];

        for (int y = 0; y < originalRows && y < newRows; y++)
        {
            for (int x = 0; x < originalColumns && x < newColumns; x++)
            {
                resizedGrid[x, y] = originalGrid[x, y];
            }
        }

        return resizedGrid;
    }
    int[,] ResizeGrid(int[,] originalGrid, int newRows, int newColumns)
    {
        if (originalGrid == null)
            return new int[newColumns, newRows];

        int originalRows = originalGrid.GetLength(1);
        int originalColumns = originalGrid.GetLength(0);
        int[,] resizedGrid = new int[newColumns, newRows];

        for (int y = 0; y < originalRows && y < newRows; y++)
        {
            for (int x = 0; x < originalColumns && x < newColumns; x++)
            {
                resizedGrid[x, y] = originalGrid[x, y];
            }
        }

        return resizedGrid;
    }

    void OnEnable()
    {
        if (fileDataHandler == null)
        {
            Awake();
            return;
        }
        GameData data = fileDataHandler.Load();
        Levels = new List<Level>();
        if (data != null)
            Levels = data.Levels;
    }
    private void Awake()
    {
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, "load.game");
    }
    void DrawColorOrderList()
    {
        // Draw color order list
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Bus Color Order", EditorStyles.boldLabel);
        if (GUILayout.Button("Add new color"))
        {
            Levels[currentItemIndex].colorList.Add(ColorType.Empty);
            // colorList[colorIndex.Length] = selectedColor;
            // colorList = colorList.Concat(new ColorType[] { ColorType.Empty }).ToArray();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        // Add button

        GUILayout.EndHorizontal();

        // Draw color list
        for (int i = 0; i < Levels[currentItemIndex].colorList.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            // Draw enum dropdown for each color
            Levels[currentItemIndex].colorList[i] = (ColorType)EditorGUILayout.EnumPopup(Levels[currentItemIndex].colorList[i]);
            // Levels[currentItemIndex].colorList = colorList;

            // Remove button
            if (GUILayout.Button("Remove"))
            {
                Levels[currentItemIndex].colorList.RemoveAt(i);
                GUILayout.EndHorizontal();
                break;
            }

            // Up button
            if (i > 0 && GUILayout.Button("▲", GUILayout.Width(20)))
            {
                var temp = Levels[currentItemIndex].colorList[i - 1];
                Levels[currentItemIndex].colorList[i - 1] = Levels[currentItemIndex].colorList[i];
                Levels[currentItemIndex].colorList[i] = temp;
            }

            // Down button
            if (i < Levels[currentItemIndex].colorList.Count - 1 && GUILayout.Button("▼", GUILayout.Width(20)))
            {
                var temp = Levels[currentItemIndex].colorList[i + 1];
                Levels[currentItemIndex].colorList[i + 1] = Levels[currentItemIndex].colorList[i];
                Levels[currentItemIndex].colorList[i] = temp;
            }

            // Draw number for each color
            GUILayout.Label((i + 1).ToString(), GUILayout.Width(20));

            GUILayout.EndHorizontal();
        }
    }
}