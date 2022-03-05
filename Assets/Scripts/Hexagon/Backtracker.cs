using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backtracker : MonoBehaviour
{
	public HexagonalGrid grid;
    public GameObject visualPlane;
    public float _delay = .01f;

	private MazeCell[,] _maze;
	private int _height, _width;
    private bool _visualMode;
    private bool _isGenerating = false;
    private int _hexagonalShape; // 0 : flat | 1 : pointy 
    private GameObject _visualPlane;

	private void Update()
	{
        if (_isGenerating == false && grid.maze != null) // Wait for HexagonalGrid to generate grid
        {
            _isGenerating = true;
            Init();
        }
	}

	void Init()
    {
        _maze = grid.maze;
		_height = grid.Height;
		_width = grid.Width;
        _visualMode = grid.visualMode;
        _hexagonalShape = grid.GridMode % 2;

        CreateMaze();
	}
    void CreateMaze()
    {
        List<MazeCell> stack = new List<MazeCell>();

        // Setting up initial cell
        int y_start = 0;
        int x_start = 0;

        if (grid.Diameter != 0)
        {
            y_start = _height / 2;
            x_start = _width / 2;
        }

        if (_visualMode)
        {
            Vector3 pos = new Vector3(x_start, .1f, y_start);
            _visualPlane = Instantiate(visualPlane, pos, Quaternion.Euler(0f, 90f * (_hexagonalShape + 1), 0f));
        }

        _maze[y_start, x_start].visited = 1;
        stack.Add(_maze[y_start, x_start]);

        StartCoroutine(BreakWalls(stack, _height, _width));
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

                if (_visualMode)
                {
                    Vector3 pos;
                    if (_hexagonalShape == 0)
                        pos = new Vector3(neighborCell.x * grid.hexWidth, .1f, neighborCell.y * grid.hexHeight + .5f * (neighborCell.x % 2));
                    else
                        pos = new Vector3(neighborCell.x * grid.hexHeight + .5f * (neighborCell.y % 2), .1f, neighborCell.y * grid.hexWidth);
                    _visualPlane.transform.position = pos;
                }
                yield return new WaitForSeconds(_delay);
            }
        }
    }

    void DestroyWalls(GameObject wall1, GameObject wall2)
    {
        Destroy(wall1);
        Destroy(wall2);
    }

    bool IsValidCell(int y, int x)
	{
        if (x < 0 || x >= _width)
            return (false);
        if (y < 0 || y >= _height)
            return (false);
        if (_maze[y, x] == null)
            return (false);
        if (_maze[y, x].visited == 1)
            return (false);
        return (true);
    }

    MazeCell CheckNeighbours(MazeCell cell, int height, int width)
    {
        List<MazeCell> list = new List<MazeCell>();
        int y = cell.y;
        int x = cell.x;

        if (IsValidCell(y, x - 1))
            list.Add(_maze[y, x - 1]);
        if (IsValidCell(y - 1, x))
            list.Add(_maze[y - 1, x]);
        if (IsValidCell(y, x + 1))
            list.Add(_maze[y, x + 1]);
        if (IsValidCell(y + 1, x))
            list.Add(_maze[y + 1, x]);

        if (_hexagonalShape == 0) // Flat
		{
            if (x % 2 == 0)
			{
                if (IsValidCell(y - 1, x + 1))
                    list.Add(_maze[y - 1, x + 1]);
                if (IsValidCell(y - 1, x - 1))
                    list.Add(_maze[y - 1, x - 1]);
            }
            else
			{
                if (IsValidCell(y + 1, x - 1))
                    list.Add(_maze[y + 1, x - 1]);
                if (IsValidCell(y + 1, x + 1))
                    list.Add(_maze[y + 1, x + 1]);
            }
        }
        else // Pointy
		{
            if (y % 2 == 0)
			{
                if (IsValidCell(y + 1, x - 1))
                    list.Add(_maze[y + 1, x - 1]);
                if (IsValidCell(y - 1, x - 1))
                    list.Add(_maze[y - 1, x - 1]);
            }
            else
			{
                if (IsValidCell(y + 1, x + 1))
                    list.Add(_maze[y + 1, x + 1]);
                if (IsValidCell(y - 1, x + 1))
                    list.Add(_maze[y - 1, x + 1]);
            }
        }
        if (list.Count == 0) // No neighbor found
            return null;

        return (list[Random.Range(0, list.Count)]);
    }
}
