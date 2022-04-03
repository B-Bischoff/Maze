using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacktrackerSquare : MonoBehaviour
{
    [Header("Grid generator reference")]
    public RectangularGrid Grid;

    [Header("Algorithm parameters")]
    private float delay = 0.01f;
    public int width, height;
    public bool visualMode;

    [Header("Prefabs")]
    public GameObject Wall;
    public GameObject Ground;
    public GameObject visualPlane;

    private BacktrackerCell[,] _maze;
    private GameObject _visualPlane;
    private bool _isGenerating;

    public class BacktrackerCell : MazeCell
    {
        public bool visited;

        public BacktrackerCell(MazeCell cell) : base(cell.x, cell.y)
        {
            this.walls = cell.walls;
            this.visited = false;
        }
    }

    BacktrackerCell[,] ConvertCellToBacktracker(MazeCell[,] maze)
    {
        BacktrackerCell[,] newMaze = new BacktrackerCell[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (maze[y, x] != null)
                    newMaze[y, x] = new BacktrackerCell(maze[y, x]);
            }
        }
        return (newMaze);
    }

    private void Update()
    {
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
        _maze =  ConvertCellToBacktracker(Grid.maze);
        CreateMaze();
    }



	void CreateMaze()
    {
        List<BacktrackerCell> stack = new List<BacktrackerCell>();

        // Setting up initial cell
        _maze[0, 0].visited = true;
        stack.Add(_maze[0, 0]);

        if (visualMode)
		{
            Vector3 pos = new Vector3(_maze[0, 0].x + .5f, .1f, _maze[0, 0].y + .5f);
            _visualPlane = Instantiate(visualPlane, pos, Quaternion.identity);
		}

        StartCoroutine(BreakWalls(stack, height, width));
    }

    IEnumerator BreakWalls(List<BacktrackerCell> stack, int height, int width)
    {
        while (stack.Count > 0)
        {
            // Getting last cell from stack
            BacktrackerCell currentCell = stack[stack.Count - 1];
            stack.Remove(currentCell);

            BacktrackerCell neighborCell = CheckNeighbours(currentCell, height, width);

            if (neighborCell != null)
            {
                // Updating stack and current cells
                stack.Add(currentCell);
                currentCell.visited = true;
                stack.Add(neighborCell);
                neighborCell.visited = true;

                // Remove walls
                Vector2 dir = new Vector2(currentCell.x - neighborCell.x, currentCell.y - neighborCell.y);

                if (dir.y == -1)
                    DestroyWalls(currentCell.walls[0], neighborCell.walls[2]);
                else if (dir.y == 1)
                    DestroyWalls(currentCell.walls[2], neighborCell.walls[0]);
                else if (dir.x == -1)
                    DestroyWalls(currentCell.walls[3], neighborCell.walls[1]);
                else
                    DestroyWalls(currentCell.walls[1], neighborCell.walls[3]);

                if (visualMode)
                {
                    Vector3 pos = new Vector3(neighborCell.x + .5f, .1f, neighborCell.y + .5f);
                    _visualPlane.transform.position = pos;
                }

                yield return new WaitForSeconds(delay);
            }
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

    void    DestroyWalls(GameObject wall1, GameObject wall2)
    {
        Destroy(wall1);
        Destroy(wall2);
    }

    BacktrackerCell CheckNeighbours(BacktrackerCell cell, int height, int width)
    {
        List<BacktrackerCell> list = new List<BacktrackerCell>();

        if (cell.y > 0 && _maze[cell.y - 1, cell.x].visited == false) // bottom
            list.Add(_maze[cell.y - 1, cell.x]);
        if (cell.y < height - 1 && _maze[cell.y + 1, cell.x].visited == false) // top
            list.Add(_maze[cell.y + 1, cell.x]);
        if (cell.x > 0 && _maze[cell.y, cell.x - 1].visited == false) // left
            list.Add(_maze[cell.y, cell.x - 1]);
        if (cell.x < width - 1 && _maze[cell.y, cell.x + 1].visited == false) // right
            list.Add(_maze[cell.y, cell.x + 1]);
        if (list.Count == 0) // No neighbor found
            return null;

        return (list[Random.Range(0, list.Count)]);
    }
}
