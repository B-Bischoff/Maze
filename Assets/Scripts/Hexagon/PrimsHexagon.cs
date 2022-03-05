using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimsHexagon : MonoBehaviour
{
    public HexagonalGrid grid;
    public GameObject visitedVisu;
    public GameObject frontierVisu;
    public float _delay = .01f;
    public bool _visualMode;

    private PrimsCell[,] _maze;
    private int _height, _width;
    private bool _isGenerating = false;
    private int _hexagonalShape; // 0 : flat | 1 : pointy 

    public class PrimsCell : MazeCell
    {
        public bool visited, inFrontier;

        public PrimsCell(MazeCell cell): base (cell.x, cell.y)
        {
            this.walls = cell.walls;
            visited = false;
            inFrontier = false;
        }
    }

    private void Update()
    {
        if (_isGenerating == false && grid.maze != null) // Wait for HexagonalGrid to generate grid
        {
            _isGenerating = true;
            Init();
        }
    }

    PrimsCell[,] ConvertCellToPrims(MazeCell[,] maze)
    {
        PrimsCell[,] newMaze = new PrimsCell[_height, _width];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (maze[y, x] != null)
                    newMaze[y, x] = new PrimsCell(maze[y, x]);
            }
        }
        return (newMaze);
    }

    void Init()
    {
        _height = grid.Height;
        _width = grid.Width;
        _visualMode = grid.visualMode;
        _hexagonalShape = grid.GridMode % 2;

        _maze = ConvertCellToPrims(grid.maze);

        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        // List initialization
        List<PrimsCell> frontierList = new List<PrimsCell>();

        // Setting up initial cell
        int y_start = 0;
        int x_start = 0;

        if (grid.Diameter != 0)
        {
            y_start = _height / 2;
            x_start = _width / 2;
        }

        _maze[y_start, x_start].visited = true;

        if (_visualMode)
        {
            if (_visualMode)
                DisplayVisual(visitedVisu, x_start, y_start, .1f);
        }

        AddNeighboursToFrontier(_maze[y_start, x_start], frontierList);

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
            yield return new WaitForSeconds(_delay);
        }
    }
    

    void AddNeighboursToFrontier(PrimsCell cell, List<PrimsCell> list)
    {
        int y = cell.y;
        int x = cell.x;

        if (IsInFrontier(y, x - 1))
            SetNeighbor(_maze[y, x - 1], list);
        if (IsInFrontier(y - 1, x))
            SetNeighbor(_maze[y - 1, x], list);
        if (IsInFrontier(y, x + 1))
            SetNeighbor(_maze[y, x + 1], list);
        if (IsInFrontier(y + 1, x))
            SetNeighbor(_maze[y + 1, x], list);

        if (_hexagonalShape == 0) // Flat
        {
            if (x % 2 == 0)
            {
                if (IsInFrontier(y - 1, x + 1))
                    SetNeighbor(_maze[y - 1, x + 1], list);
                if (IsInFrontier(y - 1, x - 1))
                    SetNeighbor(_maze[y - 1, x - 1], list);
            }
            else
            {
                if (IsInFrontier(y + 1, x - 1))
                    SetNeighbor(_maze[y + 1, x - 1], list);
                if (IsInFrontier(y + 1, x + 1))
                    SetNeighbor(_maze[y + 1, x + 1], list);
            }
        }
        else // Pointy
        {
            if (y % 2 == 0)
            {
                if (IsInFrontier(y + 1, x - 1))
                    SetNeighbor(_maze[y + 1, x - 1], list);
                if (IsInFrontier(y - 1, x - 1))
                    SetNeighbor(_maze[y - 1, x - 1], list);
            }
            else
            {
                if (IsInFrontier(y + 1, x + 1))
                    SetNeighbor(_maze[y + 1, x + 1], list);
                if (IsInFrontier(y - 1, x + 1))
                    SetNeighbor(_maze[y - 1, x + 1], list);
            }
        }
    }

    void SetNeighbor(PrimsCell cell, List<PrimsCell> list)
    {
        cell.inFrontier = true;
        list.Add(cell);

        if (_visualMode)
            DisplayVisual(frontierVisu, cell.x, cell.y, .09f);
    }

    void ConnectToNeighbor(PrimsCell cell)
    {
        List<PrimsCell> list = new List<PrimsCell>();
        int y = cell.y;
        int x = cell.x;

        if (IsVisited(y, x - 1))
            list.Add(_maze[y, x - 1]);
        if (IsVisited(y - 1, x))
            list.Add(_maze[y - 1, x]);
        if (IsVisited(y, x + 1))
            list.Add(_maze[y, x + 1]);
        if (IsVisited(y + 1, x))
            list.Add(_maze[y + 1, x]);

        if (_hexagonalShape == 0) // Flat
        {
            if (x % 2 == 0)
            {
                if (IsVisited(y - 1, x + 1))
                    list.Add(_maze[y - 1, x + 1]);
                if (IsVisited(y - 1, x - 1))
                    list.Add(_maze[y - 1, x - 1]);
            }
            else
            {
                if (IsVisited(y + 1, x - 1))
                    list.Add(_maze[y + 1, x - 1]);
                if (IsVisited(y + 1, x + 1))
                    list.Add(_maze[y + 1, x + 1]);
            }
        }
        else // Pointy
        {
            if (y % 2 == 0)
            {
                if (IsVisited(y + 1, x - 1))
                    list.Add(_maze[y + 1, x - 1]);
                if (IsVisited(y - 1, x - 1))
                    list.Add(_maze[y - 1, x - 1]);
            }
            else
            {
                if (IsVisited(y + 1, x + 1))
                    list.Add(_maze[y + 1, x + 1]);
                if (IsVisited(y - 1, x + 1))
                    list.Add(_maze[y - 1, x + 1]);
            }
        }
        if (list.Count == 0) // No neighbor found
            return ;

        if (_visualMode)
            DisplayVisual(visitedVisu, x, y, .1f);

        if (list.Count > 0)
            RemoveWalls(cell, list[Random.Range(0, list.Count)]);
    }

    void RemoveWalls(PrimsCell currentCell, PrimsCell neighborCell)
	{
        // Remove walls
        Vector2 dir = new Vector2(currentCell.x - neighborCell.x, currentCell.y - neighborCell.y);

        if (_hexagonalShape == 0) // Flat
        {
            if (dir.y == -1 && dir.x == 0)
                DestroyWalls(currentCell.walls[1], neighborCell.walls[4]);
            else if (dir.y == 1 && dir.x == 0)
                DestroyWalls(currentCell.walls[4], neighborCell.walls[1]);
            if (currentCell.x % 2 == 0)
            {
                if (dir.y == 0 && dir.x == -1)
                    DestroyWalls(currentCell.walls[2], neighborCell.walls[5]);
                else if (dir.y == 1 && dir.x == -1)
                    DestroyWalls(currentCell.walls[3], neighborCell.walls[0]);
                else if (dir.y == 1 && dir.x == 1)
                    DestroyWalls(currentCell.walls[5], neighborCell.walls[2]);
                else if (dir.y == 0 && dir.x == 1)
                    DestroyWalls(currentCell.walls[0], neighborCell.walls[3]);
            }
            else
            {
                if (dir.y == -1 && dir.x == -1)
                    DestroyWalls(currentCell.walls[2], neighborCell.walls[5]);
                else if (dir.y == -1 && dir.x == 1)
                    DestroyWalls(currentCell.walls[0], neighborCell.walls[3]);
                else if (dir.y == 0 && dir.x == 1)
                    DestroyWalls(currentCell.walls[5], neighborCell.walls[2]);
                else if (dir.y == 0 && dir.x == -1)
                    DestroyWalls(currentCell.walls[3], neighborCell.walls[0]);
            }
        }
        else // Pointy
        {
            if (dir.y == 0 && dir.x == -1)
                DestroyWalls(currentCell.walls[1], neighborCell.walls[4]);
            else if (dir.y == 0 && dir.x == 1)
                DestroyWalls(currentCell.walls[4], neighborCell.walls[1]);
            if (currentCell.y % 2 == 0)
            {
                if (dir.y == -1 && dir.x == 0)
                    DestroyWalls(currentCell.walls[0], neighborCell.walls[3]);
                else if (dir.y == 1 && dir.x == 0)
                    DestroyWalls(currentCell.walls[2], neighborCell.walls[5]);
                else if (dir.y == 1 && dir.x == 1)
                    DestroyWalls(currentCell.walls[3], neighborCell.walls[0]);
                else if (dir.y == -1 && dir.x == 1)
                    DestroyWalls(currentCell.walls[5], neighborCell.walls[2]);
            }
            else
            {
                if (dir.y == -1 && dir.x == -1)
                    DestroyWalls(currentCell.walls[0], neighborCell.walls[3]);
                else if (dir.y == 1 && dir.x == -1)
                    DestroyWalls(currentCell.walls[2], neighborCell.walls[5]);
                else if (dir.y == 1 && dir.x == 0)
                    DestroyWalls(currentCell.walls[3], neighborCell.walls[0]);
                else if (dir.y == -1 && dir.x == 0)
                    DestroyWalls(currentCell.walls[5], neighborCell.walls[2]);
            }
        }
    }

    void DestroyWalls(GameObject wall1, GameObject wall2)
    {
        Destroy(wall1);
        Destroy(wall2);
    }

    bool IsInFrontier(int y, int x)
    {
        if (x < 0 || x >= _width)
            return (false);
        if (y < 0 || y >= _height)
            return (false);
        if (_maze[y, x] == null)
            return (false);
        if (_maze[y, x].visited == true)
            return (false);
        if (_maze[y, x].inFrontier == true)
            return (false);
        return (true);
    }

    bool IsVisited(int y, int x)
    {
        if (x < 0 || x >= _width)
            return (false);
        if (y < 0 || y >= _height)
            return (false);
        if (_maze[y, x] == null)
            return (false);
        if (_maze[y, x].visited == false)
            return (false);
        return (true);
    }

    void DisplayVisual(GameObject visu, int x, int y, float altitude)
	{
        Vector3 pos;
        if (_hexagonalShape == 0)
            pos = new Vector3(x * grid.hexWidth, altitude, y * grid.hexHeight + .5f * (x % 2));
        else
            pos = new Vector3(x * grid.hexHeight + .5f * (y % 2), altitude, y * grid.hexWidth);
        Instantiate(visu, pos, Quaternion.Euler(0f, 90f * (_hexagonalShape + 1), 0f)).transform.parent = transform;
    }
}
