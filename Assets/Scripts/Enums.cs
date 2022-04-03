using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze_enums
{
    public enum MazeTypeEnum { square, hexagon };
    public enum RectangularGenerationEnum { backtracker, prims, kruskal, recursive, better_recursive };
    public enum HexagonalGenerationEnum { backtracker, prims, kruskal, better_recursive };
    public enum HexagonTypeEnum { square_flat, square_pointy, hexagon_flat, hexagon_pointy};
}
