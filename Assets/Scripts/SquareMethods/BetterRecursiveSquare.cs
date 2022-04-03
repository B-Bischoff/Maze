using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterRecursiveSquare : MonoBehaviour
{
    [Header("Grid generator reference")]
    public RectangularGrid Grid;

    [Header("Algorithm parameters")]
    public int width, height;
    public int maxDepth;
    public bool visualMode;

    [Header("Prefab")]
    public GameObject Ground;
    public GameObject vertWall, horWall;
    public GameObject planeA, planeB;

	private BetterRecurCell[,] _maze;

    public class BetterRecurCell : MazeCell
	{
        public int region;
        public int subRegion;

        public BetterRecurCell(int x, int y, int region) : base(x, y)
		{
            this.region = region;
            this.subRegion = 0;
            this.walls = new GameObject[4];
		}
	}

    void Start()
    {
        height = Grid.Height;
        width = Grid.Width;
        maxDepth = Grid.MaxDepth;
        visualMode = Grid.VisualMode;

        if (height <= 0 || width <= 0)
        {
            Debug.Log("Invalid maze length");
            return;
        }

        _maze = new BetterRecurCell[height, width];

        CreateGround();
        CreateBorders();

        // Init tab + list
        
        List<BetterRecurCell> initialRegion = new List<BetterRecurCell>();
        for (int y = 0; y < height; y++)
		{
            for (int x = 0; x < width; x++)
			{
                _maze[y, x] = new BetterRecurCell(x, y, 0);
                initialRegion.Add(_maze[y, x]);
			}
		}

        StartCoroutine(CreateMaze(initialRegion, 0));
    }

    bool IsInRegionBorders(BetterRecurCell cell, List<BetterRecurCell> regionList)
	{
        int x = cell.x;
        int y = cell.y;

        if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
            return true;

        if (!regionList.Contains(_maze[y + 1, x]))
            return true;
        else if (!regionList.Contains(_maze[y, x + 1]))
            return true;
        else if (!regionList.Contains(_maze[y - 1, x]))
            return true;
        else if (!regionList.Contains(_maze[y, x - 1]))
            return true;

        return false;
	}

    IEnumerator CreateMaze(List<BetterRecurCell> regionList, int depth)
    {
        if (depth > maxDepth)
            yield break;

        // Region indicate if the cell belongs to the region or not 
        for (int i = 0; i < regionList.Count; i++)
            regionList[i].region = 1;

        List<BetterRecurCell> expandList = new List<BetterRecurCell>();

        // Choose two random cells
        BetterRecurCell seed1 = regionList[Random.Range(0, regionList.Count)];
        while (IsInRegionBorders(seed1, regionList) == false)
            seed1 = regionList[Random.Range(0, regionList.Count)];

        BetterRecurCell seed2 = regionList[Random.Range(0, regionList.Count)];
        while (IsInRegionBorders(seed2, regionList) == false || seed2 == seed1)
            seed2 = regionList[Random.Range(0, regionList.Count)];

        // Adding seeds to expand list
        AddCellToSubregion(seed1,expandList, 1);
        AddCellToSubregion(seed2,expandList, 2);

        // Expand seeds
        while (!IsEntireRegionSplitted(regionList))
		{
            BetterRecurCell randomCell = expandList[Random.Range(0, expandList.Count)];
            expandList.Remove(randomCell);
            AddNeighbors(randomCell, expandList);
            yield return new WaitForSeconds(Grid.Delay);
        }

        // Split subregion in lists
        List<BetterRecurCell> subRegionAList = new List<BetterRecurCell>();
        List<BetterRecurCell> subRegionBList = new List<BetterRecurCell>();

        for (int i = 0; i < regionList.Count; i++)
        {
            if (regionList[i].subRegion == 2)
                subRegionAList.Add(regionList[i]);
            else
                subRegionBList.Add(regionList[i]);
        }

        // Generating walls between subregions
        List<GameObject> wallList = new List<GameObject>();
        for (int i = 0; i < subRegionAList.Count; i++)
            yield return StartCoroutine(CheckRegionBorder(subRegionAList[i], wallList));
        
        // Creating wall gap
        if (wallList.Count > 0)
            Destroy(wallList[Random.Range(0, wallList.Count)]);

        if (visualMode)
        {
            foreach (Transform child in gameObject.transform) // Delete planes
                if (child.name == "Plane1(Clone)" || child.name == "Plane2(Clone)")
                    Destroy(child.gameObject);
        }

        // Reset region properties of every cells 
        for (int i = 0; i < regionList.Count; i++)
        {
            regionList[i].region = 0;
            regionList[i].subRegion = 0;
        }

        depth++;

        if (subRegionAList.Count >= 4) // 4 is the minimum cell amount needed to create two subregions
            yield return StartCoroutine(CreateMaze(subRegionAList, depth));
        if (subRegionBList.Count >= 4)
            yield return StartCoroutine(CreateMaze(subRegionBList, depth));
    }
    
    IEnumerator CheckRegionBorder(BetterRecurCell cell, List<GameObject> wallList)
	{
        int y = cell.y;
        int x = cell.x;
        if (y > 0 && _maze[y - 1, x].subRegion == 1 && _maze[y - 1, x].region == 1) // Top
		{
            Vector3 pos = new Vector3(x + .5f, .5f, y);
            yield return StartCoroutine(ConstructWall(cell.walls[0], pos, horWall, wallList));
        }
        if (y < height - 1 && _maze[y + 1, x].subRegion == 1 && _maze[y + 1, x].region == 1) // Bottom
		{
            Vector3 pos = new Vector3(x + .5f, .5f, y + 1f);
            yield return StartCoroutine(ConstructWall(cell.walls[2], pos, horWall, wallList));
        }
        if (x > 0 && _maze[y, x - 1].subRegion == 1 && _maze[y, x - 1].region == 1) // Left
        {
            Vector3 pos = new Vector3(x, .5f, y +.5f);
            yield return StartCoroutine(ConstructWall(cell.walls[1], pos, vertWall, wallList));
        }
        if (x < width - 1 && _maze[y, x + 1].subRegion == 1 && _maze[y, x + 1].region == 1) // Right
        {
            Vector3 pos = new Vector3(x + 1f, .5f, y+.5f);
            yield return StartCoroutine(ConstructWall(cell.walls[3], pos, vertWall, wallList));
        }
    }

    IEnumerator ConstructWall(GameObject cellWall, Vector3 pos, GameObject wall, List<GameObject> wallList)
	{
        cellWall = Instantiate(wall, pos, Quaternion.identity);
        cellWall.transform.parent = gameObject.transform;
        wallList.Add(cellWall);
        yield return new WaitForSeconds(Grid.Delay);
	}

    void AddNeighbors(BetterRecurCell cell, List<BetterRecurCell> list)
	{
        int y = cell.y;
        int x = cell.x;

        if (y > 0 && _maze[y - 1, x].subRegion == 0 && _maze[y - 1, x].region == 1) // Top
            AddCellToSubregion(_maze[y - 1, x], list, cell.subRegion);
        if (y < height - 1 && _maze[y + 1, x].subRegion == 0 && _maze[y + 1, x].region == 1) // Bottom
            AddCellToSubregion(_maze[y + 1, x], list, cell.subRegion);
        if (x > 0 && _maze[y, x - 1].subRegion == 0 && _maze[y, x - 1].region == 1) // Left
            AddCellToSubregion(_maze[y, x - 1], list, cell.subRegion);
        if (x < width - 1 && _maze[y, x + 1].subRegion == 0 && _maze[y, x + 1].region == 1) // Right
            AddCellToSubregion(_maze[y, x + 1], list, cell.subRegion);
    }

    void AddCellToSubregion(BetterRecurCell cell, List<BetterRecurCell> list, int newSubregion)
	{
        cell.subRegion = newSubregion;
        list.Add(cell);
        if (visualMode)
        {
            Vector3 pos = new Vector3(cell.x + .5f, .01f, cell.y + .5f);
            if (newSubregion == 2)
                Instantiate(planeA, pos, Quaternion.identity).transform.parent = gameObject.transform;
            else
                Instantiate(planeB, pos, Quaternion.identity).transform.parent = gameObject.transform;
        }
    }

    bool IsEntireRegionSplitted(List<BetterRecurCell> region)
    {
        for (int i = 0; i < region.Count; i++)
        {
            if (region[i].subRegion == 0)
                return (false);
        }
        return (true);
    }

    void CreateBorders()
    {
        // + .5f is used to create non intersecting walls
        for (int i = 0; i < width; i++)
        {
            Vector3 bottomPos = new Vector3(i + .5f, .5f, 0);
            Vector3 topPos = new Vector3(i + .5f, .5f, height);
            Instantiate(horWall, bottomPos, Quaternion.identity).transform.parent = gameObject.transform;
            Instantiate(horWall, topPos, Quaternion.identity).transform.parent = gameObject.transform;
        }
        for (int i = 0; i < height; i++)
        {
            Vector3 leftPos = new Vector3(0, .5f, i + .5f);
            Vector3 rightPos = new Vector3(width, .5f, i + .5f);
            Instantiate(vertWall, leftPos, Quaternion.identity).transform.parent = gameObject.transform;
            Instantiate(vertWall, rightPos, Quaternion.identity).transform.parent = gameObject.transform;
        }
    }

    void CreateGround()
    {
        Vector3 groundPos = new Vector3(width / 2f, 0, height / 2f);
        Vector3 groundScale = new Vector3(width / 10f, 1, height / 10f);
        Ground.transform.localScale = groundScale;
        Instantiate(Ground, groundPos, Quaternion.identity).transform.parent = gameObject.transform;
    }
}
