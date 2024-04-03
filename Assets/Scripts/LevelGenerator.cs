using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject gridPrefab;
    private List<GameObject> gridCellList = new();
    private float space = 1.25f;

    public void Generate(Level level)
    {
        foreach (GameObject obj in gridCellList.ToList())
            Destroy(obj);

        gridCellList.Clear();

        for (int y = 0; y < level.bottomGridRowCount; y++)
        {
            for (int x = 0; x < level.bottomGridColumnCount; x++)
            {
                Transform grid = Instantiate(gridPrefab, transform).transform;
                grid.position = new Vector3((float)x * space + (level.bottomGridColumnCount / 2), -.5f, (float)y * space - 1f);
                gridCellList.Add(grid.gameObject);
            }
        }

        for (int y = 0; y < level.topGridRowCount; y++)
        {
            for (int x = 0; x < level.topGridColumnCount; x++)
            {
                Transform grid = Instantiate(gridPrefab, transform).transform;
                grid.position = new Vector3((float)x * space + (level.bottomGridColumnCount / 2), -.5f, (float)y * space - 1f);
                gridCellList.Add(grid.gameObject);
            }
        }
    }
}
