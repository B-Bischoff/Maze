using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maze_enums;

public class HexagonalGrid : MonoBehaviour
{
    public GameObject hexagon;
	public GameObject hexa_plane;


	[HideInInspector]
	public HexagonTypeEnum GridMode;

	[HideInInspector]
	public HexagonalGenerationEnum HexagonalGeneration;

	public int Radius;
	public int Height, Width;

	public int MaxDepth;

	public float Delay;

	public bool visualMode;

	public MazeCell[,] maze;

	[HideInInspector]
    public float hexHeight = 1.1f;
	[HideInInspector]
	public float hexWidth = .95f;

	void Start()
	{
		switch (GridMode)
		{
			case HexagonTypeEnum.square_flat:
				CreateSquareFlat();
				break;
			case HexagonTypeEnum.square_pointy:
				CreateSquarePointy();
				break;
			case HexagonTypeEnum.hexagon_flat:
				CreateHexFlat(Radius);
				break;
			case HexagonTypeEnum.hexagon_pointy:
				CreateHexPointy(Radius);
				break;
			default:
				Debug.Log("Wrong grid mode");
				break;
		}

		switch (HexagonalGeneration)
		{
			case HexagonalGenerationEnum.backtracker:
				GetComponent<BacktrackerHexagon>().enabled = true;
				break;
			case HexagonalGenerationEnum.prims:
				GetComponent<PrimsHexagon>().enabled = true;
				break;
			case HexagonalGenerationEnum.kruskal:
				GetComponent<KruskalHexagon>().enabled = true;
				break;
			case HexagonalGenerationEnum.better_recursive:
				GetComponent<BetterRecursiveHexagon>().enabled = true;
				break;
		}
	}

	public void CreateSquareFlat()
	{
		Radius = 0;

		// Maze initialization
		maze = new MazeCell[Height, Width];

		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				float xPos = x * hexWidth;
				float zPos = x % 2 * hexHeight / 2 + (y * hexHeight);
				Vector3 pos = new Vector3(xPos, 0f, zPos);

				GameObject walls = Instantiate(hexagon, pos, Quaternion.identity);
				walls.transform.parent = transform;

				Instantiate(hexa_plane, pos + new Vector3(0, .3f, 0), hexa_plane.transform.rotation).transform.parent = transform;

				// Cell initialization
				maze[y, x] = new MazeCell(x, y);
				AssignWallToMazeCell(maze[y, x], walls);
			}
		}
	}

	public void CreateSquarePointy()
	{
		Radius = 0;

		// Maze initialization
		maze = new MazeCell[Height, Width];

		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				float xPos = y % 2 * (hexHeight) / 2 + (x * (hexHeight));
				float zPos = y * hexWidth;
				Vector3 pos = new Vector3(xPos, 0, zPos);
				
				GameObject walls = Instantiate(hexagon, pos, Quaternion.Euler(0f, 90f, 0f));
				walls.transform.parent = transform;

				Instantiate(hexa_plane, pos + new Vector3(0, .3f, 0), Quaternion.identity).transform.parent = transform;

				// Cell initialization
				maze[y, x] = new MazeCell(x, y);
				AssignWallToMazeCell(maze[y, x], walls);
			}
		}
	}

	public void CreateHexFlat(int radius)
	{
		Height = radius * 2 + 1;
		Width = radius * 2 + 1;

		// Array initialization
		maze = new MazeCell[radius * 2 + 1, radius * 2 + 1];

		for (int y = 0; y < radius * 2 + 1; y++)
		{
			for (int x = 0; x < radius * 2 + 1; x++)
			{
				int nbr = radius - 1 - (2 * y) + radius % 2; // Calculating the left dead zone
				int nbr2 = radius - (2 * ((radius * 2) - y)) - radius % 2; // Calculating the right dead zone

				if (x < nbr || radius * 2 - x < nbr)
					continue;
				if (x < nbr2 || radius * 2 - x < nbr2)
					continue;
	
				float xPos = x * hexWidth;
				float zPos = x % 2 * hexHeight / 2 + (y * hexHeight);

				Vector3 pos = new Vector3(xPos, 0, zPos);

				GameObject walls = Instantiate(hexagon, pos, Quaternion.identity);
				walls.transform.parent = transform;

				Instantiate(hexa_plane, pos + new Vector3(0, .3f, 0), hexa_plane.transform.rotation).transform.parent = transform;

				// Cell initialization
				maze[y, x] = new MazeCell(x, y);
				AssignWallToMazeCell(maze[y, x], walls);
			}
		}
	}

	public void CreateHexPointy(int radius)
	{
		Height = radius * 2 + 1;
		Width = radius * 2 + 1;

		// Array initialization
		maze = new MazeCell[radius * 2 + 1, radius * 2 + 1];

		for (int y = 0; y < radius * 2 + 1; y++)
		{
			for (int x = 0; x < radius * 2 + 1; x++)
			{
				int nbr = radius - 1 - (2 * x) + radius % 2; // Calculating the left dead zone
				int nbr2 = radius - (2 * ((radius * 2) - x)) - radius % 2; // Calculating the right dead zone

				if (y < nbr || radius * 2 - y < nbr)
					continue;
				if (y < nbr2 || radius * 2 - y < nbr2)
					continue;

				float xPos = y % 2 * (hexHeight) / 2 + (x * (hexHeight));
				float zPos = y * hexWidth;
				Vector3 pos = new Vector3(xPos, 0, zPos);

				GameObject walls = Instantiate(hexagon, pos, Quaternion.Euler(0f, 90f, 0f));
				walls.transform.parent = transform;

				Instantiate(hexa_plane, pos + new Vector3(0, .3f, 0), Quaternion.identity).transform.parent = transform;

				// Cell initialization
				maze[y, x] = new MazeCell(x, y);
				AssignWallToMazeCell(maze[y, x], walls);
			}
		}
	}

	public void AssignWallToMazeCell(MazeCell cell, GameObject walls)
	{
		// Walls array initialization
		cell.walls = new GameObject[6];

		cell.walls[0] = walls.transform.Find("Cylinder.000").gameObject;
		cell.walls[1] = walls.transform.Find("Cylinder.001").gameObject;
		cell.walls[2] = walls.transform.Find("Cylinder.002").gameObject;
		cell.walls[3] = walls.transform.Find("Cylinder.003").gameObject;
		cell.walls[4] = walls.transform.Find("Cylinder.004").gameObject;
		cell.walls[5] = walls.transform.Find("Cylinder.005").gameObject;
	}
}