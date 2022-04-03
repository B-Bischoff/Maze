using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KruskalHexagon : MonoBehaviour
{
    public HexagonalGrid grid;
    public GameObject visualPlane;
    public float _delay = .01f;
    public bool _visualMode;

    private KruskalCell[,] _maze;
    private int _height, _width;
    private bool _isGenerating = false;
    private int _hexagonalShape; // 0 : flat | 1 : pointy 

    public class KruskalCell : MazeCell
    {
        public int set;
        public GameObject plane;
        public Color color;

        public KruskalCell(MazeCell cell, int set, GameObject visualPlane, GameObject parent, int _hexagonalShape, float hexWidth, float hexHeight) : base (cell.x, cell.y)
        {
            this.walls = cell.walls;
            this.set = set;
            Vector3 pos;
            if (_hexagonalShape == 0)
                pos = new Vector3(x * hexWidth, .31f, y * hexHeight + .5f * (x % 2));
            else
                pos = new Vector3(x * hexHeight + .5f * (y % 2), .31f, y * hexWidth);
            this.plane = Instantiate(visualPlane, pos, Quaternion.Euler(0f, 90f * (_hexagonalShape + 1), 0f));
            this.plane.transform.parent = parent.transform;
            this.color = new Color(
                Random.Range(0, 256) / 255f,
                Random.Range(0, 256) / 255f,
                Random.Range(0, 256) / 255f);
            this.plane.GetComponent<Renderer>().material.color = this.color;
        }
        public KruskalCell(MazeCell cell, int set) : base (cell.x, cell.y)
        {
            this.walls = cell.walls;
            this.set = set;
        }
    }
    private void Update()
    {
        _delay = grid.Delay;
        if (_isGenerating == false && grid.maze != null) // Wait for HexagonalGrid to generate grid
        {
            _isGenerating = true;
            Init();
        }
    }

    KruskalCell[,] ConvertCellToKruskal(MazeCell[,] maze)
    {
        KruskalCell[,] newMaze = new KruskalCell[_height, _width];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (maze[y, x] != null)
				{
                    if (_visualMode)
                        newMaze[y, x] = new KruskalCell(maze[y, x], y * _width + x, visualPlane, gameObject, _hexagonalShape, grid.hexWidth, grid.hexHeight);
                    else
                        newMaze[y, x] = new KruskalCell(maze[y, x], y * _width + x);
                }
            }
        }
        return (newMaze);
    }


    void Init()
    {
        _height = grid.Height;
        _width = grid.Width;
        _visualMode = grid.visualMode;
        _hexagonalShape = ((int)grid.GridMode) % 2;

        _maze = ConvertCellToKruskal(grid.maze);

        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        while (!IsOnlyOneSet())
        {
            // Choosing a random cell
            int y = Random.Range(0, _height);
            int x = Random.Range(0, _width);

            if (_maze[y, x] == null)
                continue; // Choose another random cell
            KruskalCell cell = _maze[y, x];

            KruskalCell neighborCell = GetNeighborCell(cell);
            if (neighborCell != null)
            {
                AssignNewSet(cell, neighborCell);
                RemoveWalls(cell, neighborCell);
                yield return new WaitForSeconds(_delay);
            }
        }
    }

    void RemoveWalls(KruskalCell currentCell, KruskalCell neighborCell)
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

    KruskalCell GetNeighborCell(KruskalCell cell)
    {
        List<KruskalCell> list = new List<KruskalCell>();
        int y = cell.y;
        int x = cell.x;

        if (IsValidCell(y, x - 1, cell.set))
            list.Add(_maze[y, x - 1]);
        if (IsValidCell(y - 1, x, cell.set))
            list.Add(_maze[y - 1, x]);
        if (IsValidCell(y, x + 1, cell.set))
            list.Add(_maze[y, x + 1]);
        if (IsValidCell(y + 1, x, cell.set))
            list.Add(_maze[y + 1, x]);

        if (_hexagonalShape == 0) // Flat
        {
            if (x % 2 == 0)
            {
                if (IsValidCell(y - 1, x + 1, cell.set))
                    list.Add(_maze[y - 1, x + 1]);
                if (IsValidCell(y - 1, x - 1, cell.set))
                    list.Add(_maze[y - 1, x - 1]);
            }
            else
            {
                if (IsValidCell(y + 1, x - 1, cell.set))
                    list.Add(_maze[y + 1, x - 1]);
                if (IsValidCell(y + 1, x + 1, cell.set))
                    list.Add(_maze[y + 1, x + 1]);
            }
        }
        else // Pointy
        {
            if (y % 2 == 0)
            {
                if (IsValidCell(y + 1, x - 1, cell.set))
                    list.Add(_maze[y + 1, x - 1]);
                if (IsValidCell(y - 1, x - 1, cell.set))
                    list.Add(_maze[y - 1, x - 1]);
            }
            else
            {
                if (IsValidCell(y + 1, x + 1, cell.set))
                    list.Add(_maze[y + 1, x + 1]);
                if (IsValidCell(y - 1, x + 1, cell.set))
                    list.Add(_maze[y - 1, x + 1]);
            }
        }
        if (list.Count == 0) // No neighbor found
            return null;

        return (list[Random.Range(0, list.Count)]);
    }

    bool IsValidCell(int y, int x, int set)
    {
        if (x < 0 || x >= _width)
            return (false);
        if (y < 0 || y >= _height)
            return (false);
        if (_maze[y, x] == null)
            return (false);
        if (_maze[y, x].set == set)
            return (false);
        return (true);
    }

    void AssignNewSet(KruskalCell cell1, KruskalCell cell2)
    {
        int newSet;
        int oldSet;
        Color newColor;

        if (cell1.set < cell2.set)
        {
            newSet = cell1.set;
            oldSet = cell2.set;
            newColor = cell1.color;
        }
        else
        {
            newSet = cell2.set;
            oldSet = cell1.set;
            newColor = cell2.color;
        }
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_maze[y, x] != null && _maze[y, x].set == oldSet)
                {
                    _maze[y, x].set = newSet;
                    if (_visualMode)
                    {
                        _maze[y, x].plane.GetComponent<Renderer>().material.color = newColor;
                        _maze[y, x].color = newColor;
                    }
                }
            }
        }
    }

    bool IsOnlyOneSet()
    {
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                if (_maze[y, x] != null &&_maze[y, x].set != _maze[_height / 2, _width / 2].set)
                    return (false);
        return (true);
    }
}
