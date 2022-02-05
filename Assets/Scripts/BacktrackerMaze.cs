using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacktrackerMaze : MonoBehaviour
{
    [Header("Algorithm parameters")]
    public float delay = 0.01f;
    public int width, height;

    [Header("Prefabs")]
    public GameObject Wall;
    public GameObject Ground;

    private MazeCell[,] _maze;

    public class MazeCell
    {
        public int visited;
        public GameObject TopWall, LeftWall, BottomWall, RightWall;
        public int x, y;

        public MazeCell(int x, int y)
        {
            visited = 0;
            this.x = x;
            this.y = y;
        }
    }

    void Start()
    {
        if (height <= 0 || width <= 0)
        {
            Debug.Log("Invalid maze length");
            return;
        }
        _maze = new MazeCell[height, width];
        CreateMaze(width, height);
    }

    void CreateMaze(int width, int height)
    {
        List<MazeCell> stack = new List<MazeCell>();

        // Array initialization
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                _maze[y, x] = new MazeCell(x, y);

        // Creating ground
        Vector3 groundPos = new Vector3(width / 2f, 0, height / 2f);
        Vector3 groundScale = new Vector3(width / 10f, 1, height / 10f);
        Ground.transform.localScale = groundScale;
        Instantiate(Ground, groundPos, Quaternion.identity);

        // Creating every walls
        for (int i = 0; i < height; i += 1)
        {
            for (int j = 0; j < width; j += 1)
            {
                Vector3 pos = new Vector3(j + Wall.transform.localScale.x / 2, (float)Wall.transform.localScale.y / 2, i + Wall.transform.localScale.z / 2);
                GameObject wall = Instantiate(Wall, pos, Quaternion.identity);
                wall.transform.parent = gameObject.transform;
                _maze[i, j].TopWall = wall.transform.Find("TopWall").gameObject;
                _maze[i, j].LeftWall = wall.transform.Find("LeftWall").gameObject;
                _maze[i, j].BottomWall = wall.transform.Find("BottomWall").gameObject;
                _maze[i, j].RightWall = wall.transform.Find("RightWall").gameObject;
            }
        }

        // Setting up initial cell
        _maze[0, 0].visited = 1;
        stack.Add(_maze[0, 0]);

        StartCoroutine(BreakWalls(stack, height, width));
    }

    IEnumerator BreakWalls(List<MazeCell> stack, int height, int width)
    {
        while (stack.Count > 0)
        {
            // Getting last cell from stack
            MazeCell currentCell = stack[stack.Count - 1];
            stack.Remove(currentCell);

            MazeCell neighborCell = CheckNeighbours(currentCell, height, width);

            if (neighborCell != null)
            {
                // Updating stack and current cells
                stack.Add(currentCell);
                currentCell.visited = 1;
                stack.Add(neighborCell);
                neighborCell.visited = 1;

                // Remove walls
                Vector2 dir = new Vector2(currentCell.x - neighborCell.x, currentCell.y - neighborCell.y);

                if (dir.y == -1)
                    DestroyWalls(currentCell.TopWall, neighborCell.BottomWall);
                else if (dir.y == 1)
                    DestroyWalls(currentCell.BottomWall, neighborCell.TopWall);
                else if (dir.x == -1)
                    DestroyWalls(currentCell.RightWall, neighborCell.LeftWall);
                else
                    DestroyWalls(currentCell.LeftWall, neighborCell.RightWall);
                yield return new WaitForSeconds(delay);
            }
        }

        // Removing colliding walls
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (j < width - 1 && _maze[i, j].RightWall != null && _maze[i, j + 1].LeftWall != null)
                    Destroy(_maze[i, j].RightWall);
                if (i < height - 1 && _maze[i, j].TopWall != null && _maze[i + 1, j].BottomWall != null)
                    Destroy(_maze[i, j].TopWall);
            }
        }
    }

    void    DestroyWalls(GameObject wall1, GameObject wall2)
    {
        Destroy(wall1);
        Destroy(wall2);
    }

    MazeCell CheckNeighbours(MazeCell cell, int height, int width)
    {
        List<MazeCell> list = new List<MazeCell>();

        if (cell.y > 0 && _maze[cell.y - 1, cell.x].visited == 0) // bottom
            list.Add(_maze[cell.y - 1, cell.x]);
        if (cell.y < height - 1 && _maze[cell.y + 1, cell.x].visited == 0) // top
            list.Add(_maze[cell.y + 1, cell.x]);
        if (cell.x > 0 && _maze[cell.y, cell.x - 1].visited == 0) // left
            list.Add(_maze[cell.y, cell.x - 1]);
        if (cell.x < width - 1 && _maze[cell.y, cell.x + 1].visited == 0) // right
            list.Add(_maze[cell.y, cell.x + 1]);
        if (list.Count == 0) // No neighbor found
            return null;

        /*
        // Messing around with randomness 
        if (list.Count == 3)
            return list[1];
        // end messing around
        */

        return (list[Random.Range(0, list.Count)]);
    }
}
