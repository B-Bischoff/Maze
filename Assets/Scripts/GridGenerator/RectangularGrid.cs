using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maze_enums;

public class RectangularGrid : MonoBehaviour
{
    public GameObject CellPrefab;
    public GameObject GroundPrefab;

    public int Height, Width;
    public int MaxDepth;
    public bool RandomChamber;
    public bool VisualMode;
    public float Delay;

    public MazeCell[,] maze;

    [HideInInspector]
    public RectangularGenerationEnum RectangularGeneration;

    void Start()
    {
        if (Height <= 0 || Width <= 0)
        {
            Debug.Log("Invalid maze length");
            return;
        }

        switch (RectangularGeneration)
		{
            case RectangularGenerationEnum.backtracker:
                GetComponent<BacktrackerSquare>().enabled = true;
                break;
            case RectangularGenerationEnum.prims:
                GetComponent<PrimsSquare>().enabled = true;
                break;
            case RectangularGenerationEnum.kruskal:
                GetComponent<KruskalSquare>().enabled = true;
                break;
            case RectangularGenerationEnum.recursive:
                GetComponent<RecursiveSquare>().enabled = true;
                return;
            case RectangularGenerationEnum.better_recursive:
                GetComponent<BetterRecursiveSquare>().enabled = true;
                return;
        }

        maze = new MazeCell[Height, Width];

        // Array initialization
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                maze[y, x] = new MazeCell(x, y);

        // Creating ground
        Vector3 groundPos = new Vector3(Width / 2f, 0, Height / 2f);
        Vector3 groundScale = new Vector3(Width / 10f, 1, Height / 10f);
        GameObject ground = Instantiate(GroundPrefab, groundPos, Quaternion.identity);
        ground.transform.localScale = groundScale;

        // Creating every walls
        for (int i = 0; i < Height; i += 1)
        {
            for (int j = 0; j < Width; j += 1)
            {
                Vector3 pos = new Vector3(
                    j + CellPrefab.transform.localScale.x / 2,
                    (float)CellPrefab.transform.localScale.y / 2,
                    i + CellPrefab.transform.localScale.z / 2);
                GameObject cell = Instantiate(CellPrefab, pos, Quaternion.identity);
                cell.transform.parent = gameObject.transform;
                maze[i, j].walls = new GameObject[4];
                maze[i, j].walls[0] = cell.transform.Find("TopWall").gameObject;
                maze[i, j].walls[1] = cell.transform.Find("LeftWall").gameObject;
                maze[i, j].walls[2] = cell.transform.Find("BottomWall").gameObject;
                maze[i, j].walls[3] = cell.transform.Find("RightWall").gameObject;
            }
        }
    }
}
