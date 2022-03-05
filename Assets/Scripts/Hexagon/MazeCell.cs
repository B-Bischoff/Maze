using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell
{
	public GameObject[] walls;
	public int x, y;

	public MazeCell(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}
