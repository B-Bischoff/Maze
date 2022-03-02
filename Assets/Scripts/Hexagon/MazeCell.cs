using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
