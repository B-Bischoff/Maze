using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backtracker : MonoBehaviour
{
	public HexagonalGrid grid;

	private MazeCell[,] _maze;
	private int _height, _width;
    private bool _visualMode;
    private float _delay = .01f;
    private bool _isGenerating;
    private int _hexagonalShape; // 0 : flat | 1 : pointy 

	private void Start()
	{
        _isGenerating = false;
	}

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

    void Test(MazeCell cell)
	{
        foreach (GameObject go in cell.walls)
            Destroy(go);
	}

    void CreateMaze()
    {
        List<MazeCell> stack = new List<MazeCell>();

        // Setting up initial cell
        if (grid.Diameter != 0)
        {
            _maze[_height / 2, _width / 2].visited = 1;
            stack.Add(_maze[_height / 2, _width / 2]);
        }
        else
		{
            _maze[0, 0].visited = 1;
            stack.Add(_maze[0, 0]);
        }

        if (_visualMode)
        {
            //Vector3 pos = new Vector3(_maze[0, 0].x + .5f, .1f, _maze[0, 0].y + .5f);
            //_visualPlane = Instantiate(visualPlane, pos, Quaternion.identity);
        }

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

                Debug.Log("current cell y " + currentCell.y + " x " + currentCell.x);

                // Remove walls<
                Vector2 dir = new Vector2(currentCell.x - neighborCell.x, currentCell.y - neighborCell.y);

                if (_hexagonalShape == 0) // Flat
				{
                    if (currentCell.x % 2 == 0)
                    {
                        if (dir.y == -1 && dir.x == 0)
                            DestroyWalls(currentCell.walls[1], neighborCell.walls[4]);
                        else if (dir.y == 0 && dir.x == -1)
                            DestroyWalls(currentCell.walls[2], neighborCell.walls[5]);
                        else if (dir.y == 1 && dir.x == -1)
                            DestroyWalls(currentCell.walls[3], neighborCell.walls[0]);
                        else if (dir.y == 1 && dir.x == 0)
                            DestroyWalls(currentCell.walls[4], neighborCell.walls[1]);
                        else if (dir.y == 1 && dir.x == 1)
                            DestroyWalls(currentCell.walls[5], neighborCell.walls[2]);
                        else if (dir.y == 0 && dir.x == 1)
                            DestroyWalls(currentCell.walls[0], neighborCell.walls[3]);
                    }
                    else
					{
                        if (dir.y == -1 && dir.x == -1)
                            DestroyWalls(currentCell.walls[2], neighborCell.walls[5]);
                        else if (dir.y == -1 && dir.x == 0)
                            DestroyWalls(currentCell.walls[1], neighborCell.walls[4]);
                        else if (dir.y == -1 && dir.x == 1)
                            DestroyWalls(currentCell.walls[0], neighborCell.walls[3]);
                        else if (dir.y == 0 && dir.x == 1)
                            DestroyWalls(currentCell.walls[5], neighborCell.walls[2]);
                        else if (dir.y == 1 && dir.x == 0)
                            DestroyWalls(currentCell.walls[4], neighborCell.walls[1]);
                        else if (dir.y == 0 && dir.x == -1)
                            DestroyWalls(currentCell.walls[3], neighborCell.walls[0]);
                    }
                }
                else // Pointy
				{
                    if (currentCell.y % 2 == 0)
                    {
                        if (dir.y == -1 && dir.x == 0)
                            DestroyWalls(currentCell.walls[0], neighborCell.walls[3]);
                        else if (dir.y == 0 && dir.x == -1)
                            DestroyWalls(currentCell.walls[1], neighborCell.walls[4]);
                        else if (dir.y == 1 && dir.x == 0)
                            DestroyWalls(currentCell.walls[2], neighborCell.walls[5]);
                        else if (dir.y == 1 && dir.x == 1)
                            DestroyWalls(currentCell.walls[3], neighborCell.walls[0]);
                        else if (dir.y == 0 && dir.x == 1)
                            DestroyWalls(currentCell.walls[4], neighborCell.walls[1]);
                        else if (dir.y == -1 && dir.x == 1)
                            DestroyWalls(currentCell.walls[5], neighborCell.walls[2]);
                    }
                    else
                    {
                        if (dir.y == -1 && dir.x == -1)
                            DestroyWalls(currentCell.walls[0], neighborCell.walls[3]);
                        else if (dir.y == 0 && dir.x == -1)
                            DestroyWalls(currentCell.walls[1], neighborCell.walls[4]);
                        else if (dir.y == 1 && dir.x == -1)
                            DestroyWalls(currentCell.walls[2], neighborCell.walls[5]);
                        else if (dir.y == 1 && dir.x == 0)
                            DestroyWalls(currentCell.walls[3], neighborCell.walls[0]);
                        else if (dir.y == 0 && dir.x == 1)
                            DestroyWalls(currentCell.walls[4], neighborCell.walls[1]);
                        else if (dir.y == -1 && dir.x == 0)
                            DestroyWalls(currentCell.walls[5], neighborCell.walls[2]);
                    }
                }

                if (_visualMode)
                {
                    //Vector3 pos = new Vector3(neighborCell.x + .5f, .1f, neighborCell.y + .5f);
                    //_visualPlane.transform.position = pos;
                }

                yield return new WaitForSeconds(_delay);
            }
        }

        // Removing colliding walls
        /*
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
        */
    }

    void DestroyWalls(GameObject wall1, GameObject wall2)
    {
        Destroy(wall1);
        Destroy(wall2);
    }

    MazeCell CheckNeighbours(MazeCell cell, int height, int width)
    {
        List<MazeCell> list = new List<MazeCell>();
        int y = cell.y;
        int x = cell.x;

        if (_hexagonalShape == 0) // Flat
		{
            if (x % 2 == 0)
			{
                if (y < height - 1 && _maze[y + 1, x] != null && _maze[y + 1, x].visited == 0)
                    list.Add(_maze[y + 1, x]);
                if (x < width - 1 && _maze[y, x + 1] != null && _maze[y, x + 1].visited == 0)
                    list.Add(_maze[y, x + 1]);
                if (y > 0 && x < width - 1 && _maze[y - 1, x + 1] != null && _maze[y - 1, x + 1].visited == 0)
                    list.Add(_maze[y - 1, x + 1]);
                if (y > 0 && _maze[y - 1, x] != null && _maze[y - 1, x].visited == 0)
                    list.Add(_maze[y - 1, x]);
                if (y > 0 && x > 0 && _maze[y - 1, x - 1] != null && _maze[y - 1, x - 1].visited == 0)
                    list.Add(_maze[y - 1, x - 1]);
                if (x > 0 && _maze[y, x - 1] != null && _maze[y, x - 1].visited == 0)
                    list.Add(_maze[y, x - 1]);
            }
            else
			{
                if (y < height - 1 && x > 0 && _maze[y + 1, x - 1] != null && _maze[y + 1, x - 1].visited == 0)
                    list.Add(_maze[y + 1, x - 1]);
                if (y < height - 1 && _maze[y + 1, x] != null && _maze[y + 1, x].visited == 0)
                    list.Add(_maze[y + 1, x]);
                if (y < height - 1 && x < width - 1 && _maze[y + 1, x + 1] != null && _maze[y + 1, x + 1].visited == 0)
                    list.Add(_maze[y + 1, x + 1]);
                if (x > 0 && _maze[y, x - 1] != null && _maze[y, x - 1].visited == 0)
                    list.Add(_maze[y, x - 1]);
                if (x < width - 1 && _maze[y, x + 1] != null && _maze[y, x + 1].visited == 0)
                    list.Add(_maze[y, x + 1]);
                if (y > 0 && _maze[y - 1, x] != null && _maze[y - 1, x].visited == 0)
                    list.Add(_maze[y - 1, x]);
            }

        }
        else // Pointy
		{
            if (y % 2 == 0)
			{
                if (y < height - 1 && x > 0 && _maze[y + 1, x - 1] != null && _maze[y + 1, x - 1].visited == 0)
                    list.Add(_maze[y + 1, x - 1]);
                if (y < height - 1 && _maze[y + 1, x] != null && _maze[y + 1, x].visited == 0)
                    list.Add(_maze[y + 1, x]);
                if (x < width - 1 && _maze[y, x + 1] != null && _maze[y, x + 1].visited == 0)
                    list.Add(_maze[y, x + 1]);
                if (y > 0 && _maze[y - 1, x] != null && _maze[y - 1, x].visited == 0)
                    list.Add(_maze[y - 1, x]);
                if (x > 0 && y > 0 && _maze[y - 1, x - 1] != null && _maze[y - 1, x - 1].visited == 0)
                    list.Add(_maze[y - 1, x - 1]);
                if (x > 0 && _maze[y, x - 1] != null && _maze[y, x - 1].visited == 0)
                    list.Add(_maze[y, x - 1]);
            }
            else
			{
                if (y < height - 1 && _maze[y + 1, x] != null &&  _maze[y + 1, x].visited == 0)
                    list.Add(_maze[y + 1, x]);
                if (y < height - 1 && x < width - 1 && _maze[y + 1, x + 1] != null && _maze[y + 1, x + 1].visited == 0)
                    list.Add(_maze[y + 1, x + 1]);
                if (x < width - 1 && _maze[y, x + 1] != null && _maze[y, x + 1].visited == 0)
                    list.Add(_maze[y, x + 1]);
                if (y > 0 &&  x < width - 1 && _maze[y - 1, x + 1] != null && _maze[y - 1, x + 1].visited == 0)
                    list.Add(_maze[y - 1, x + 1]);
                if (y > 0 && _maze[y - 1, x] != null && _maze[y - 1, x].visited == 0)
                    list.Add(_maze[y - 1, x]);
                if (x > 0 && _maze[y, x - 1] != null && _maze[y, x - 1].visited == 0)
                    list.Add(_maze[y, x - 1]);
            }
        }

        if (list.Count == 0) // No neighbor found
            return null;

        return (list[Random.Range(0, list.Count)]);
    }
}
