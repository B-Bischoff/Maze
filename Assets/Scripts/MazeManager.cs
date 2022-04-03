using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maze_enums;

public class MazeManager : MonoBehaviour
{
    [Header("Generation type")]
    public MazeTypeEnum MazeType;

    public HexagonTypeEnum HexagonType;

    public RectangularGenerationEnum RectangularGeneration;
    public HexagonalGenerationEnum HexagonalGeneration;

    [Header("Maze parameters")]
    public int Height;
    public int Width;
    public int Radius;

    public bool VisualMode;

    public int RecursiveMaxDepth;
    public bool RecursiveRandomChamber;

    public float Delay;


    private GameObject RectangularGenerator, HexagonalGenerator; 

    void Start()
    {
        ReadPlayerPrefs();

        if (DataFormatError())
		{
            Debug.LogError("Incorrect parameters");
            return;
		}

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
            SetRectMazeProperties();
        }
        else if (MazeType == MazeTypeEnum.hexagon)
        {
            HexagonalGenerator.SetActive(true);
            SetHexMazeProperties();
        }
    }
	public void Update()
	{
        // Update the delay along the simulation
        if (MazeType == MazeTypeEnum.square)
            RectangularGenerator.GetComponent<RectangularGrid>().Delay = Delay;
        else if (MazeType == MazeTypeEnum.hexagon)
            HexagonalGenerator.GetComponent<HexagonalGrid>().Delay = Delay;
    }
    public void SetRectMazeProperties()
    {
        RectangularGenerator.GetComponent<RectangularGrid>().RectangularGeneration = RectangularGeneration;
        RectangularGenerator.GetComponent<RectangularGrid>().Height = Height;
        RectangularGenerator.GetComponent<RectangularGrid>().Width = Width;
        RectangularGenerator.GetComponent<RectangularGrid>().MaxDepth = RecursiveMaxDepth;
        RectangularGenerator.GetComponent<RectangularGrid>().RandomChamber = RecursiveRandomChamber;
        RectangularGenerator.GetComponent<RectangularGrid>().VisualMode = VisualMode;
    }

    private void SetHexMazeProperties()
	{
        HexagonalGenerator.GetComponent<HexagonalGrid>().GridMode = HexagonType;
        HexagonalGenerator.GetComponent<HexagonalGrid>().HexagonalGeneration = HexagonalGeneration;
        HexagonalGenerator.GetComponent<HexagonalGrid>().Radius = Radius;
        HexagonalGenerator.GetComponent<HexagonalGrid>().Height = Height;
        HexagonalGenerator.GetComponent<HexagonalGrid>().Width = Width;
        HexagonalGenerator.GetComponent<HexagonalGrid>().visualMode = VisualMode;
        HexagonalGenerator.GetComponent<HexagonalGrid>().MaxDepth = RecursiveMaxDepth;
    }

    bool DataFormatError()
    {
        if (MazeType == MazeTypeEnum.hexagon && HexagonType == HexagonTypeEnum.hexagon_flat || HexagonType == HexagonTypeEnum.hexagon_pointy)
		{
            if (Radius <= 0)
                return true;
		}
        else if (Height < 0 || Width < 0 || Radius < 0)
            return true;

        return false;
    }

    private void ReadPlayerPrefs()
	{

        Height = PlayerPrefs.GetInt("Height");
        Width = PlayerPrefs.GetInt("Width");
        Radius = PlayerPrefs.GetInt("Radius");
        RecursiveMaxDepth =  PlayerPrefs.GetInt("MaxDepth");
        RecursiveRandomChamber = System.Convert.ToBoolean(PlayerPrefs.GetInt("RandomChamber"));
        MazeType = (MazeTypeEnum)PlayerPrefs.GetInt("MazeType");
        HexagonType = (HexagonTypeEnum)PlayerPrefs.GetInt("HexagonType");
        RectangularGeneration = (RectangularGenerationEnum)PlayerPrefs.GetInt("RectangularGeneration");
        HexagonalGeneration = (HexagonalGenerationEnum)PlayerPrefs.GetInt("HexagonalGeneration");
        VisualMode = System.Convert.ToBoolean(PlayerPrefs.GetInt("VisualMode"));
    }
}

