using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KruskalSquare : MonoBehaviour
{
    [Header("Grid generator reference")]
    public RectangularGrid Grid;

    [Header("Algorithm parameters")]
    public float delay = 0.01f;
    private int width, height;
    private bool visualMode;

    [Header("Prefabs")]
    public GameObject Wall;
    public GameObject Ground;
    public GameObject visualPlane;

    private bool _isGenerating;

    private KruskalCell[,] _maze;

    public class KruskalCell : MazeCell
    {
        public int set;
        public GameObject plane;
        public Color color;

        public KruskalCell(MazeCell cell, int set) : base(cell.x, cell.y)
		{
            this.walls = cell.walls;
            this.set = set;
		}
        public KruskalCell(MazeCell cell, int set, GameObject visualPlane, GameObject parent) : base(cell.x, cell.y)
        {
            this.walls = cell.walls;
            this.set = set;
            Vector3 pos = new Vector3(x + .5f, .1f, y + .5f);

            // Generating plane and a random plane color 
            this.plane = Instantiate(visualPlane, pos, Quaternion.identity);
            this.plane.transform.parent = parent.transform;
            this.color = new Color(
                Random.Range(0, 256) / 255f,
                Random.Range(0, 256) / 255f,
                Random.Range(0, 256) / 255f);
            this.plane.GetComponent<Renderer>().material.color = this.color;
        }
    }

    KruskalCell[,] ConvertCellToKruskal(MazeCell[,] maze)
    {
        KruskalCell[,] newMaze = new KruskalCell[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (visualMode)
                    newMaze[y, x] = new KruskalCell(maze[y, x], width * y + x, visualPlane, gameObject);
                else
                    newMaze[y, x] = new KruskalCell(maze[y, x], width * y + x);
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
        visualMode = Grid.VisualMode;
        _maze = ConvertCellToKruskal(Grid.maze);
        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        while (!IsOnlyOneSet())
		{
            int y = Random.Range(0, height);
            int x = Random.Range(0, width);
            KruskalCell cell = _maze[y, x];

            KruskalCell neighborCell = GetNeighborCell(cell);
            if (neighborCell != null)
			{
                AssignNewSet(cell, neighborCell);
                RemoveWalls(cell, neighborCell);
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

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (_maze[y, x].set == oldSet)
                {
                    _maze[y, x].set = newSet;
                    if (visualMode)
                    {
                        _maze[y, x].plane.GetComponent<Renderer>().material.color = newColor;
                        _maze[y, x].color = newColor;
                    }
                }
            }
        }
    }

    void RemoveWalls(KruskalCell cell1, KruskalCell cell2)
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

    KruskalCell GetNeighborCell(KruskalCell cell)
	{
        List<KruskalCell> list = new List<KruskalCell>();

        int y = cell.y;
        int x = cell.x;

        if (y > 0 && cell.set != _maze[y - 1, x].set) // Bottom
            list.Add(_maze[y - 1, x]);
        if (y < height - 1 && cell.set != _maze[y + 1, x].set) // Top
            list.Add(_maze[y + 1, x]);
        if (x > 0 && cell.set != _maze[y, x - 1].set) // Left
            list.Add(_maze[y, x - 1]);
        if (x < width - 1 && cell.set != _maze[y, x + 1].set) // Right
            list.Add(_maze[y, x + 1]);

        if (list.Count == 0)
            return (null);
        return (list[Random.Range(0, list.Count)]);
    }

    bool IsOnlyOneSet()
	{
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (_maze[y, x].set != _maze[0, 0].set)
                    return (false);
        return (true);
    }
}
