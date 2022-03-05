using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prims : MonoBehaviour
{
    /*
     * Choose a random cell in the maze
     * Add every neighbours cells in the frontier list
     * Choose a random cell from the frontier list
     * Connect it with a random neighbor visited cell
     *
    */
    [Header("Algorithm parameters")]
    public int width;
    public int height;
    public float delay = 0.025f;
    public bool visualMode;

    [Header("Prefabs")]
    public GameObject Wall;
    public GameObject Ground;
    public GameObject frontierVisu;
    public GameObject visitedVisu;

    private MazeCell[,] _maze;

    public class MazeCell
    {
        public bool visited, inFrontier;
        public GameObject TopWall, LeftWall, BottomWall, RightWall;
        public int x, y;

        public MazeCell(int x, int y)
        {
            visited = false;
            inFrontier = false;
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
        CreateGound();
        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
	{
        // Array initialization
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                _maze[y, x] = new MazeCell(x, y);

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

        // List initialization
        List<MazeCell> frontierList = new List<MazeCell>();

        // Choosing a random starting cell in the maze
        int cell_x = Random.Range(0, width);
        int cell_y = Random.Range(0, height);
        _maze[cell_y, cell_x].visited = true;
        if (visualMode)
        {
            Vector3 pos = new Vector3(cell_x + .5f, .1f, cell_y + .5f);
            Instantiate(visitedVisu, pos, Quaternion.identity);
        }

        AddNeighboursToFrontier(_maze[cell_y, cell_x], frontierList);

        while (frontierList.Count > 0)
        {
            // Pick random cell from the list 
            MazeCell frontierCell = frontierList[Random.Range(0, frontierList.Count)];
            // Update cell properties
            frontierCell.visited = true;
            frontierCell.inFrontier = false;
            // Update list
            frontierList.Remove(frontierCell);
            // Select visited neighbor
            ConnectToNeighbor(frontierCell);
            AddNeighboursToFrontier(frontierCell, frontierList);
            yield return new WaitForSeconds(delay);
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

    void ConnectToNeighbor(MazeCell cell)
	{
        List<MazeCell> neighbors = new List<MazeCell>();
        int x = cell.x;
        int y = cell.y;

        // Finding a visited neighbor
        if (cell.y > 0 && _maze[y - 1, x].visited) // Top
            neighbors.Add(_maze[y - 1, x]);
        if (cell.y < height - 1 && _maze[y + 1, x].visited) // Bottom
            neighbors.Add(_maze[y + 1, x]);
        if (cell.x > 0 && _maze[y, x - 1].visited) // Left
            neighbors.Add(_maze[y, x - 1]);
        if (cell.x < width - 1 && _maze[y, x + 1].visited) // Right
            neighbors.Add(_maze[y, x + 1]);

        if (visualMode)
        {
            Vector3 pos = new Vector3(x + .5f, .1f, y + .5f);
            Instantiate(visitedVisu, pos, Quaternion.identity);
        }

        RemoveWalls(cell, neighbors[Random.Range(0, neighbors.Count)]);
    }

    void RemoveWalls(MazeCell cell1, MazeCell cell2)
	{
        Vector2 dir = new Vector2(cell1.x - cell2.x, cell1.y - cell2.y);

        if (dir.y == -1)
            DestroyWalls(cell1.TopWall, cell2.BottomWall);
        else if (dir.y == 1)
            DestroyWalls(cell1.BottomWall, cell2.TopWall);
        else if (dir.x == -1)
            DestroyWalls(cell1.RightWall, cell2.LeftWall);
        else
            DestroyWalls(cell1.LeftWall, cell2.RightWall);
    }

    void DestroyWalls(GameObject wall1, GameObject wall2)
	{
        Destroy(wall1);
        Destroy(wall2);
	}

    void AddNeighboursToFrontier(MazeCell cell, List<MazeCell> list)
	{
        int y = cell.y;
        int x = cell.x;

        if (cell.y > 0 && !_maze[y - 1, x].visited && !_maze[y - 1, x].inFrontier) // Top
            SetNeighbor(_maze[y - 1, x], list);
        if (cell.y < height - 1 && !_maze[y + 1, x].visited && !_maze[y + 1, x].inFrontier) // Bottom
            SetNeighbor(_maze[y + 1, x], list);
        if (cell.x > 0 && !_maze[y, x - 1].visited && !_maze[y, x - 1].inFrontier) // Left
            SetNeighbor(_maze[y, x - 1], list);
        if (cell.x < width - 1 && !_maze[y, x + 1].visited && !_maze[y, x + 1].inFrontier) // Right
            SetNeighbor(_maze[y, x + 1], list);
    }

    void SetNeighbor(MazeCell cell, List<MazeCell> list)
	{
        cell.inFrontier = true;
        list.Add(cell);

        if (visualMode)
        {
            Vector3 pos = new Vector3(cell.x + .5f, .1f, cell.y + .5f);
            Instantiate(frontierVisu, pos, Quaternion.identity);
        }
    }

    void CreateGound()
    {
        Vector3 groundPos = new Vector3(width / 2f, 0, height / 2f);
        Vector3 groundScale = new Vector3(width / 10f, 1, height / 10f);
        Ground.transform.localScale = groundScale;
        Instantiate(Ground, groundPos, Quaternion.identity).transform.parent = gameObject.transform;
    }
}
