using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonalGrid : MonoBehaviour
{
    public GameObject hexa_pointy;
    public GameObject hexa_flat;

	[Header("0 : Square flat | 1 : Square pointy | 2 : Hexagon flat | 3 : Hexagon pointy")]	
	public int GridMode;

	public int Diameter;
	public int Height, Width;

	[HideInInspector]
	public MazeCell[,] maze;

    private float hexHeight = 1.1f;
    private float hexWidth = .95f;

	public class MazeCell
	{
		public int visited;
		public GameObject[] walls;
		public int x, y;

		public MazeCell(int x, int y)
		{
			visited = 0;
			this.x = x;
			this.y = y;
		}
	}

	void Start()
	{
		switch (GridMode)
		{
			case 0:
				CreateSquareFlat(hexHeight, hexWidth, Height, Width);
				break;
			case 1:
				CreateSquarePointy(hexWidth, hexHeight, Height, Width);
				break;
			case 2:
				CreateHexFlat(hexHeight, Diameter);
				break;
			case 3:
				CreateHexPointy(hexHeight, Diameter);
				break;
			default:
				Debug.Log("Wrong grid mode");
				break;
		}
	}

	public void CreateSquareFlat(float hexHeight, float hexWidth, int height, int width)
	{
		// Maze initialization
		maze = new MazeCell[height, width];

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float xPos = x * hexWidth;
				float zPos = x % 2 * hexHeight / 2 + (y * hexHeight);
				Vector3 pos = new Vector3(xPos, 0, zPos);

				GameObject walls = Instantiate(hexa_flat, pos, Quaternion.identity);
				walls.transform.parent = transform;

				// Cell initialization
				maze[y, x] = new MazeCell(x, y);
				AssignWallToMazeCell(maze[y, x], walls);
			}
		}
	}

	public void CreateSquarePointy(float hexHeight, float hexWidth, int height, int width)
	{
		// Maze initialization
		maze = new MazeCell[height, width];

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float xPos = y % 2 * (hexWidth) / 2 + (x * (hexWidth));
				float zPos = y * hexHeight;
				Vector3 pos = new Vector3(xPos, 0, zPos);
				
				GameObject walls = Instantiate(hexa_pointy, pos, Quaternion.identity);
				walls.transform.parent = transform;

				// Cell initialization
				maze[y, x] = new MazeCell(x, y);
				AssignWallToMazeCell(maze[y, x], walls);
			}
		}
	}

	public void CreateHexFlat(float hexHeight, int radius)
	{
		// Array initialization
		maze = new MazeCell[radius * 2 + 1, radius * 2 + 1];

		for (int q = radius; q >= -radius; q--)
		{
			int r1 = Mathf.Max(-radius, -q - radius);
			int r2 = Mathf.Min(radius, -q + radius);
			for (int r = r1; r <= r2; r++)
			{
				Vector3 pos = new Vector3((-q - r) * .95f, 0, (q - r) * (hexHeight / 2f));
				GameObject walls = Instantiate(hexa_flat, pos, Quaternion.identity);

				walls.transform.parent = transform;

				// Converting world position to array position
				int x = q + radius;
				int y = r + x;

				// Cell initialization
				maze[y, x] = new MazeCell(x, y);
				AssignWallToMazeCell(maze[y, x], walls);
			}
		}
	}

	public void CreateHexPointy(float hexHeight, int radius)
	{
		// Array initialization
		maze = new MazeCell[radius * 2 + 1, radius * 2 + 1];

		for (int q = -radius; q <= radius; q++)
		{
			int r1 = Mathf.Max(-radius, -q - radius);
			int r2 = Mathf.Min(radius, -q + radius);
			for (int r = r1; r <= r2; r++)
			{
				Vector3 pos = new Vector3((q - r) * (hexHeight / 2f), 0, (-q - r) * .95f);

				GameObject walls = Instantiate(hexa_pointy, pos, Quaternion.identity);
				walls.transform.parent = transform;

				// Converting world position to array position
				int x = q + radius;
				int y = r + x;

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

		cell.walls[0] = walls.transform.Find("Cylinder.001").gameObject;
		cell.walls[1] = walls.transform.Find("Cylinder.002").gameObject;
		cell.walls[2] = walls.transform.Find("Cylinder.003").gameObject;
		cell.walls[3] = walls.transform.Find("Cylinder.004").gameObject;
		cell.walls[4] = walls.transform.Find("Cylinder.005").gameObject;
		cell.walls[5] = walls.transform.Find("Cylinder.006").gameObject;
	}
}