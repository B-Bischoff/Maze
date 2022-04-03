using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimsSquare : MonoBehaviour
{
    [Header("Grid generator reference")]
    public RectangularGrid Grid;

    [Header("Algorithm parameters")]
    private int width;
    private int height;
    private float delay = 0.025f;
    private bool visualMode;

    [Header("Prefabs")]
    public GameObject Wall;
    public GameObject Ground;
    public GameObject frontierVisu;
    public GameObject visitedVisu;

    private PrimsCell[,] _maze;

    private bool _isGenerating;
    
    public class PrimsCell : MazeCell
	{
        public bool visited;
        public bool inFrontier;

        public PrimsCell(MazeCell cell) : base(cell.x, cell.y)
		{
            this.walls = cell.walls;
            this.visited = false;
            this.inFrontier = false;
		}
	}

    PrimsCell[,] ConvertCellToPrims(MazeCell[,] maze)
    {
        PrimsCell[,] newMaze = new PrimsCell[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (maze[y, x] != null)
                    newMaze[y, x] = new PrimsCell(maze[y, x]);
            }
        }
        return (newMaze);
    }

    private void Update()
    {
        delay = Grid.Delay;
        if (_isGenerating == false && Grid.maze != null) // Wait for RectangularGrid to generate grid
        {
            _isGenerating = true;
            Init();
        }
    }

    private void Init()
    {
        height = Grid.Height;
        width = Grid.Width;
        visualMode = Grid.VisualMode;
        _maze = ConvertCellToPrims(Grid.maze);
        StartCoroutine(CreateMaze());
    }


    IEnumerator CreateMaze()
	{
        // List initialization
        List<PrimsCell> frontierList = new List<PrimsCell>();

        // Choosing a random starting cell in the maze
        int cell_x = Random.Range(0, width);
        int cell_y = Random.Range(0, height);
        _maze[cell_y, cell_x].visited = true;
        if (visualMode)
        {
            Vector3 pos = new Vector3(cell_x + .5f, .11f, cell_y + .5f);
            Instantiate(visitedVisu, pos, Quaternion.identity);
        }

        AddNeighboursToFrontier(_maze[cell_y, cell_x], frontierList);

        while (frontierList.Count > 0)
        {
            // Pick random cell from the list 
            PrimsCell frontierCell = frontierList[Random.Range(0, frontierList.Count)];
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
                if (j < width - 1 && _maze[i, j].walls[3] != null && _maze[i, j + 1].walls[1] != null)
                    Destroy(_maze[i, j].walls[3]);
                if (i < height - 1 && _maze[i, j].walls[0] != null && _maze[i + 1, j].walls[2] != null)
                    Destroy(_maze[i, j].walls[0]);
            }
        }
    }

    void ConnectToNeighbor(PrimsCell cell)
	{
        List<PrimsCell> neighbors = new List<PrimsCell>();
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
            Vector3 pos = new Vector3(x + .5f, .11f, y + .5f);
            Instantiate(visitedVisu, pos, Quaternion.identity);
        }

        RemoveWalls(cell, neighbors[Random.Range(0, neighbors.Count)]);
    }

    void RemoveWalls(PrimsCell cell1, PrimsCell cell2)
	{
        Vector2 dir = new Vector2(cell1.x - cell2.x, cell1.y - cell2.y);

        if (dir.y == -1)
            DestroyWalls(cell1.walls[0], cell2.walls[2]);
        else if (dir.y == 1)
            DestroyWalls(cell1.walls[2], cell2.walls[0]);
        else if (dir.x == -1)
            DestroyWalls(cell1.walls[3], cell2.walls[1]);
        else
            DestroyWalls(cell1.walls[1], cell2.walls[3]);
    }

    void DestroyWalls(GameObject wall1, GameObject wall2)
	{
        Destroy(wall1);
        Destroy(wall2);
	}

    void AddNeighboursToFrontier(PrimsCell cell, List<PrimsCell> list)
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

    void SetNeighbor(PrimsCell cell, List<PrimsCell> list)
	{
        cell.inFrontier = true;
        list.Add(cell);

        if (visualMode)
        {
            Vector3 pos = new Vector3(cell.x + .5f, .1f, cell.y + .5f);
            Instantiate(frontierVisu, pos, Quaternion.identity);
        }
    }
}
