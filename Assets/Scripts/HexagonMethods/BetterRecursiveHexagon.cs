using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterRecursiveHexagon : MonoBehaviour
{
    public HexagonalGrid grid;
    public GameObject hexagonPrefab;
    public GameObject planeA, planeB;
    public int maxDepth;
    public float _delay = .01f;

    private BetterRecurCell[,] _maze;
    private bool _isGenerating = false;
    private int _height, _width;
    private bool _visualMode;
    private int _hexagonalShape; // 0 : flat | 1 : pointy 

    public class BetterRecurCell: MazeCell
    {
        public int region;
        public int subRegion;

        public BetterRecurCell(MazeCell cell, int region): base (cell.x, cell.y)
        {
            this.region = region;
            subRegion = 0;
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

    BetterRecurCell[,] ConvertMazeCellToRecur(MazeCell[,] maze, List<BetterRecurCell> list)
    {
        BetterRecurCell[,] newMaze = new BetterRecurCell[_height, _width];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++) 
            {
                if (maze[y, x] != null)
                {
                    RemoveNonBorderWalls(maze[y, x]);
                    newMaze[y, x] = new BetterRecurCell(maze[y, x], 0);
                    list.Add(newMaze[y, x]);
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
        maxDepth = grid.MaxDepth;
        _hexagonalShape = ((int)grid.GridMode) % 2;

        List<BetterRecurCell> initialRegion = new List<BetterRecurCell>();

        _maze = ConvertMazeCellToRecur(grid.maze, initialRegion);


		StartCoroutine(CreateMaze(initialRegion, 0));
    }

    bool IsValidCell(int y, int x)
    {
        if (x < 0 || x >= _width)
            return (false);
        if (y < 0 || y >= _height)
            return (false);
        if (grid.maze[y, x] == null)
            return (false);
        return (true);
    }

    bool IsInRegionAndSubRegion(int y, int x)
    {
        if (x < 0 || x >= _width)
            return (false);
        if (y < 0 || y >= _height)
            return (false);
        if (_maze[y, x] == null)
            return (false);
        if (_maze[y, x].subRegion != 1)
            return (false);
        if (_maze[y, x].region != 1)
            return (false);
        return (true);
    }

    bool IsInRegion(int y, int x)
    {
        if (x < 0 || x >= _width)
            return (false);
        if (y < 0 || y >= _height)
            return (false);
        if (_maze[y, x] == null)
            return (false);
        if (_maze[y, x].subRegion != 0)
            return (false);
        if (_maze[y, x].region != 1)
            return (false);
        return (true);
    }

    public bool IsInBorders(BetterRecurCell cell, List<BetterRecurCell> region)
	{
        int y = cell.y;
        int x = cell.x;

        if (y == 0 || x == 0 || x == _width - 1 || y == _height - 1)
            return true;
        return true;

        // return false;
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
        while (IsInBorders(seed1, regionList) == false)
            seed1 = regionList[Random.Range(0, regionList.Count)];
        BetterRecurCell seed2 = regionList[Random.Range(0, regionList.Count)];
        while (IsInBorders(seed2, regionList) == false || seed1 == seed2)
            seed2 = regionList[Random.Range(0, regionList.Count)];


        // Adding seeds to expand list
        AddCellToSubregion(seed1, expandList, 1);
        AddCellToSubregion(seed2, expandList, 2);

        // Expand seeds
        while (!IsEntireRegionSplitted(regionList))
        {
            BetterRecurCell randomCell = expandList[Random.Range(0, expandList.Count)];
            expandList.Remove(randomCell);
            AddNeighbors(randomCell, expandList);
            yield return new WaitForSeconds(_delay);
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
		{
            int i = Random.Range(0, wallList.Count);
            Destroy(wallList[i]);
            if (i % 2 == 0)
                Destroy(wallList[i + 1]);
            else
                Destroy(wallList[i - 1]);
        }

        if (_visualMode)
        {
            foreach (Transform child in gameObject.transform) // Delete planes
                if (child.name == "HexagonPlane 1(Clone)" || child.name == "HexagonPlane 2(Clone)")
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

        if (_hexagonalShape == 0) // Flat
        {
            if (IsInRegionAndSubRegion(y - 1, x))
            {
                CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.004").gameObject, 0, wallList);
                CreateWall(_maze[y - 1, x], hexagonPrefab.transform.Find("Cylinder.001").gameObject, 0, wallList);
            }
            if (IsInRegionAndSubRegion(y + 1, x))
            {
                CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.001").gameObject, 0, wallList);
                CreateWall(_maze[y + 1, x], hexagonPrefab.transform.Find("Cylinder.004").gameObject, 0, wallList);
            }
            if (x % 2 == 0)
            {
                if (IsInRegionAndSubRegion(y, x + 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.002").gameObject, 0, wallList);
                    CreateWall(_maze[y, x + 1], hexagonPrefab.transform.Find("Cylinder.005").gameObject, 0, wallList);
                }
                if (IsInRegionAndSubRegion(y, x - 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.000").gameObject, 0, wallList);
                    CreateWall(_maze[y, x - 1], hexagonPrefab.transform.Find("Cylinder.003").gameObject, 0, wallList);
                }
                if (IsInRegionAndSubRegion(y - 1, x + 1))
				{
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.003").gameObject, 0, wallList);
                    CreateWall(_maze[y - 1, x + 1], hexagonPrefab.transform.Find("Cylinder.000").gameObject, 0, wallList);
                }
                if (IsInRegionAndSubRegion(y - 1, x - 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.005").gameObject, 0, wallList);
                    CreateWall(_maze[y - 1, x - 1], hexagonPrefab.transform.Find("Cylinder.002").gameObject, 0, wallList);
                }
            }
            else
            {
                if (IsInRegionAndSubRegion(y, x - 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.005").gameObject, 0, wallList);
                    CreateWall(_maze[y, x - 1], hexagonPrefab.transform.Find("Cylinder.002").gameObject, 0, wallList);
                }
                if (IsInRegionAndSubRegion(y, x + 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.003").gameObject, 0, wallList);
                    CreateWall(_maze[y, x + 1], hexagonPrefab.transform.Find("Cylinder.000").gameObject, 0, wallList);
                }
                if (IsInRegionAndSubRegion(y + 1, x - 1))
				{
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.000").gameObject, 0, wallList);
                    CreateWall(_maze[y + 1, x - 1], hexagonPrefab.transform.Find("Cylinder.003").gameObject, 0, wallList);
                }
                if (IsInRegionAndSubRegion(y + 1, x + 1))
				{
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.002").gameObject, 0, wallList);
                    CreateWall(_maze[y + 1, x + 1], hexagonPrefab.transform.Find("Cylinder.005").gameObject, 0, wallList);
                }
            }
        }
        else // Pointy
        {
            if (IsInRegionAndSubRegion(y, x - 1))
            {
                CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.004").gameObject, 90f, wallList);
                CreateWall(_maze[y, x - 1], hexagonPrefab.transform.Find("Cylinder.001").gameObject, 90f, wallList);
            }
            if (IsInRegionAndSubRegion(y, x + 1))
            {
                CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.001").gameObject, 90f, wallList);
                CreateWall(_maze[y, x + 1], hexagonPrefab.transform.Find("Cylinder.004").gameObject, 90f, wallList);
            }
            if (y % 2 == 0)
            {
                if (IsInRegionAndSubRegion(y + 1, x))
				{
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.000").gameObject, 90f, wallList);
                    CreateWall(_maze[y + 1, x], hexagonPrefab.transform.Find("Cylinder.003").gameObject, 90f, wallList);
                }
                if (IsInRegionAndSubRegion(y - 1, x))
				{
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.002").gameObject, 90f, wallList);
                    CreateWall(_maze[y - 1, x], hexagonPrefab.transform.Find("Cylinder.005").gameObject, 90f, wallList);
                }
                if (IsInRegionAndSubRegion(y - 1, x - 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.003").gameObject, 90f, wallList);
                    CreateWall(_maze[y - 1, x - 1], hexagonPrefab.transform.Find("Cylinder.000").gameObject, 90f, wallList);
                }
                if (IsInRegionAndSubRegion(y + 1, x - 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.005").gameObject, 90f, wallList);
                    CreateWall(_maze[y + 1, x - 1], hexagonPrefab.transform.Find("Cylinder.002").gameObject, 90f, wallList);
                }
            }
            else
            {
                if (IsInRegionAndSubRegion(y + 1, x))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.005").gameObject, 90f, wallList);
                    CreateWall(_maze[y + 1, x], hexagonPrefab.transform.Find("Cylinder.002").gameObject, 90f, wallList);
                }
                if (IsInRegionAndSubRegion(y - 1, x))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.003").gameObject, 90f, wallList);
                    CreateWall(_maze[y - 1, x], hexagonPrefab.transform.Find("Cylinder.000").gameObject, 90f, wallList);
                }
                if (IsInRegionAndSubRegion(y - 1, x + 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.002").gameObject, 90f, wallList);
                    CreateWall(_maze[y - 1, x + 1], hexagonPrefab.transform.Find("Cylinder.005").gameObject, 90f, wallList);
                }
                if (IsInRegionAndSubRegion(y + 1, x + 1))
                {
                    CreateWall(cell, hexagonPrefab.transform.Find("Cylinder.000").gameObject, 90f, wallList);
                    CreateWall(_maze[y + 1, x + 1], hexagonPrefab.transform.Find("Cylinder.003").gameObject, 90f, wallList);
                }
            }
        }
        yield return null;
    }

    void AddCellToSubregion(BetterRecurCell cell, List<BetterRecurCell> list, int newSubregion)
    {
        cell.subRegion = newSubregion;
        list.Add(cell);
        if (_visualMode)
        {
            Vector3 pos;
            if (_hexagonalShape == 0)
                pos = new Vector3(cell.x * grid.hexWidth, .31f, cell.y * grid.hexHeight + .5f * (cell.x % 2));
            else
                pos = new Vector3(cell.x * grid.hexHeight + .5f * (cell.y % 2), .31f, cell.y * grid.hexWidth);

            if (newSubregion == 2)
                Instantiate(planeA, pos, Quaternion.Euler(0f, 90f * (_hexagonalShape + 1), 0f)).transform.parent = transform;
            else
                Instantiate(planeB, pos, Quaternion.Euler(0f, 90f * (_hexagonalShape + 1), 0f)).transform.parent = transform;
        }
    }
    void CreateWall(BetterRecurCell cell, GameObject wall, float angle, List<GameObject> walls)
    {
        Vector3 pos;
        GameObject CreatedWall;

        wall.transform.localScale = new Vector3(.7f, .7f, .7f);

        if (_hexagonalShape == 0)
            pos = new Vector3(cell.x * grid.hexWidth, 1f, cell.y * grid.hexHeight + .5f * (cell.x % 2));
        else
            pos = new Vector3(cell.x * grid.hexHeight + .5f * (cell.y % 2), 1f, cell.y * grid.hexWidth);
        CreatedWall = Instantiate(wall, pos, Quaternion.Euler(0, wall.transform.eulerAngles.y + angle, 0));
        CreatedWall.transform.parent = transform;
        walls.Add(CreatedWall);
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

    void DestroyNeigborsWall(MazeCell currentCell, MazeCell neighborCell)
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
   

    void AddNeighbors(BetterRecurCell cell, List<BetterRecurCell> list)
	{
        int y = cell.y;
        int x = cell.x;

        if (IsInRegion(y, x - 1))
            AddCellToSubregion(_maze[y, x - 1], list, cell.subRegion);
        if (IsInRegion(y - 1, x))
            AddCellToSubregion(_maze[y - 1, x], list, cell.subRegion);
        if (IsInRegion(y, x + 1))
            AddCellToSubregion(_maze[y, x + 1], list, cell.subRegion);
        if (IsInRegion(y + 1, x))
            AddCellToSubregion(_maze[y + 1, x], list, cell.subRegion);

        if (_hexagonalShape == 0) // Flat
        {
            if (x % 2 == 0)
            {
                if (IsInRegion(y - 1, x + 1))
                    AddCellToSubregion(_maze[y - 1, x + 1], list, cell.subRegion);
                if (IsInRegion(y - 1, x - 1))
                    AddCellToSubregion(_maze[y - 1, x - 1], list, cell.subRegion);
            }
            else
            {
                if (IsInRegion(y + 1, x - 1))
                    AddCellToSubregion(_maze[y + 1, x - 1], list, cell.subRegion);
                if (IsInRegion(y + 1, x + 1))
                    AddCellToSubregion(_maze[y + 1, x + 1], list, cell.subRegion);
            }
        }
        else // Pointy
        {
            if (y % 2 == 0)
            {
                if (IsInRegion(y + 1, x - 1))
                    AddCellToSubregion(_maze[y + 1, x - 1], list, cell.subRegion);
                if (IsInRegion(y - 1, x - 1))
                    AddCellToSubregion(_maze[y - 1, x - 1], list, cell.subRegion);
            }
            else
            {
                if (IsInRegion(y + 1, x + 1))
                    AddCellToSubregion(_maze[y + 1, x + 1], list, cell.subRegion);
                if (IsInRegion(y - 1, x + 1))
                    AddCellToSubregion(_maze[y - 1, x + 1], list, cell.subRegion);
            }
        }
    }

    void RemoveNonBorderWalls(MazeCell cell)
    {
        int y = cell.y;
        int x = cell.x;

        if (IsValidCell(y, x - 1))
            DestroyNeigborsWall(cell, grid.maze[y, x - 1]);
        if (IsValidCell(y - 1, x))
            DestroyNeigborsWall(cell, grid.maze[y - 1, x]);
        if (IsValidCell(y, x + 1))
            DestroyNeigborsWall(cell, grid.maze[y, x + 1]);
        if (IsValidCell(y + 1, x))
            DestroyNeigborsWall(cell, grid.maze[y + 1, x]);

        if (_hexagonalShape == 0) // Flat
        {
            if (x % 2 == 0)
            {
                if (IsValidCell(y - 1, x + 1))
                    DestroyNeigborsWall(cell, grid.maze[y - 1, x + 1]);
                if (IsValidCell(y - 1, x - 1))
                    DestroyNeigborsWall(cell, grid.maze[y - 1, x - 1]);
            }
            else
            {
                if (IsValidCell(y + 1, x - 1))
                    DestroyNeigborsWall(cell, grid.maze[y + 1, x - 1]);
                if (IsValidCell(y + 1, x + 1))
                    DestroyNeigborsWall(cell, grid.maze[y + 1, x + 1]);
            }
        }
        else // Pointy
        {
            if (y % 2 == 0)
            {
                if (IsValidCell(y + 1, x - 1))
                    DestroyNeigborsWall(cell, grid.maze[y + 1, x - 1]);
                if (IsValidCell(y - 1, x - 1))
                    DestroyNeigborsWall(cell, grid.maze[y - 1, x - 1]);
            }
            else
            {
                if (IsValidCell(y + 1, x + 1))
                    DestroyNeigborsWall(cell, grid.maze[y + 1, x + 1]);
                if (IsValidCell(y - 1, x + 1))
                    DestroyNeigborsWall(cell, grid.maze[y - 1, x + 1]);
            }
        }
    }

    void DestroyWalls(GameObject wall1, GameObject wall2)
	{
		if (wall1 != null)
			Destroy(wall1);
        if (wall2 != null)
		    Destroy(wall2);
    }
}
