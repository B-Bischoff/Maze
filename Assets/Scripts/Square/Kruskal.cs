using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kruskal : MonoBehaviour
{
    [Header("Algorithm parameters")]
    public float delay = 0.01f;
    public int width, height;
    public bool visualMode;

    [Header("Prefabs")]
    public GameObject Wall;
    public GameObject Ground;
    public GameObject visualPlane;

    private MazeCell[,] _maze;

    public class MazeCell
    {
        public GameObject TopWall, LeftWall, BottomWall, RightWall;
        public int x, y;
        public int set;
        public GameObject plane;
        public Color color;

        public MazeCell(int x, int y, int set, GameObject visualPlane, GameObject parent)
        {
            this.x = x;
            this.y = y;
            this.set = set;
            Vector3 pos = new Vector3(x + .5f, .1f, y + .5f);
            this.plane = Instantiate(visualPlane, pos, Quaternion.identity);
            this.plane.transform.parent = parent.transform;
            this.color = new Color(
                Random.Range(0, 256) / 255f,
                Random.Range(0, 256) / 255f,
                Random.Range(0, 256) / 255f);
            this.plane.GetComponent<Renderer>().material.color = this.color;
        }
        public MazeCell(int x, int y, int set)
		{
            this.x = x;
            this.y = y;
            this.set = set;
        }
    }

    void Start()
    {
        if (height <= 0 || width <= 0)
        {
            Debug.Log("Invalid maze length");
            return;
        }
        CreateGround();
        _maze = new MazeCell[height, width];
        StartCoroutine(CreateMaze());
    }
    IEnumerator CreateMaze()
    {
        // Array initialization
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (visualMode)
                    _maze[y, x] = new MazeCell(x, y, height * y + x, visualPlane, gameObject);
                else
                    _maze[y, x] = new MazeCell(x, y, height * y + x);
            }
        }

        // Creating every walls
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
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

        while (!IsOnlyOneSet())
		{
            int y = Random.Range(0, height);
            int x = Random.Range(0, width);
            MazeCell cell = _maze[y, x];
            
            MazeCell neighborCell = GetNeighborCell(cell);
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
                if (j < width - 1 && _maze[i, j].RightWall != null && _maze[i, j + 1].LeftWall != null)
                    Destroy(_maze[i, j].RightWall);
                if (i < height - 1 && _maze[i, j].TopWall != null && _maze[i + 1, j].BottomWall != null)
                    Destroy(_maze[i, j].TopWall);
            }
        }
    }

    void AssignNewSet(MazeCell cell1, MazeCell cell2)
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

    MazeCell GetNeighborCell(MazeCell cell)
	{
        List<MazeCell> list = new List<MazeCell>();

        int y = cell.y;
        int x = cell.x;

        if (y > 0 && cell.set != _maze[y - 1, x].set) // Top
            list.Add(_maze[y - 1, x]);
        if (y < height - 1 && cell.set != _maze[y + 1, x].set) // Bottom
            list.Add(_maze[y + 1, x]);
        if (x > 0 && cell.set != _maze[y, x - 1].set) // Left
            list.Add(_maze[y, x - 1]);
        if (x < width - 1 && cell.set != _maze[y, x + 1].set) // Top
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

    void CreateGround()
	{
        Vector3 groundPos = new Vector3(width / 2f, 0, height / 2f);
        Vector3 groundScale = new Vector3(width / 10f, 1, height / 10f);
        Ground.transform.localScale = groundScale;
        Instantiate(Ground, groundPos, Quaternion.identity).transform.parent = gameObject.transform;
    }
}
