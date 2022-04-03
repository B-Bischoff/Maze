using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maze_enums;

public class MazeManager : MonoBehaviour
{

    public MazeTypeEnum MazeType;

    public HexagonTypeEnum HexagonType;

    public RectangularGenerationEnum RectangularGeneration;
    public HexagonalGenerationEnum HexagonalGeneration;


    private GameObject RectangularGenerator, HexagonalGenerator; 

    void Start()
    {
        RectangularGenerator = transform.Find("RectangularMazeGenerator").gameObject;
        if (RectangularGenerator == null)
		{
            Debug.LogError("Rectangular maze generator not found");
            return;
		}

        HexagonalGenerator = transform.Find("HexagonalMazeGenerator").gameObject;
        if (HexagonalGenerator == null)
        {
            Debug.LogError("Rectangular maze generator not found");
            return;
        }

        if (MazeType == MazeTypeEnum.square)
        {
            RectangularGenerator.SetActive(true);
            RectangularGenerator.GetComponent<RectangularGrid>().RectangularGeneration = RectangularGeneration;
        }
        else if (MazeType == MazeTypeEnum.hexagon)
        {
            HexagonalGenerator.SetActive(true);
            HexagonalGenerator.GetComponent<HexagonalGrid>().GridMode = HexagonType;
            HexagonalGenerator.GetComponent<HexagonalGrid>().HexagonalGeneration = HexagonalGeneration;
        }
    }
}
