using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maze_enums;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("UI elements")]
    public GameObject HexagonStyle;
    public GameObject RectGenMethod, HexGenMethod;
    public GameObject WidthUi, HeightUi, RadiusUi;
    public GameObject MaxDepthUi, RandomRoomUi;
    public GameObject VisualModeUi;

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

	private void Start()
	{
        VisualMode = true;
        RecursiveRandomChamber = true;

        MazeType = MazeTypeEnum.square;
    }

	private void Update()
	{
        // Display : HexagonStyle and relevant generation methods
        if (MazeType == MazeTypeEnum.square)
        {
            HexagonStyle.SetActive(false);
            HexGenMethod.SetActive(false);
            RectGenMethod.SetActive(true);
        }
        else
        {
            HexagonStyle.SetActive(true);
            HexGenMethod.SetActive(true);
            RectGenMethod.SetActive(false);
        }
        
        // Display : Height, Width and radius
        if (MazeType == MazeTypeEnum.square || HexagonType == HexagonTypeEnum.square_flat || HexagonType == HexagonTypeEnum.square_pointy)
		{
            RadiusUi.SetActive(false);
            WidthUi.SetActive(true);
            HeightUi.SetActive(true);
		}
		else
        {
            RadiusUi.SetActive(true);
            WidthUi.SetActive(false);
            HeightUi.SetActive(false);
        }

        // Display random room UI
        if (MazeType == MazeTypeEnum.square && RectangularGeneration == RectangularGenerationEnum.recursive)
            RandomRoomUi.SetActive(true);
        else
            RandomRoomUi.SetActive(false);

        // Display max depth UI
        if (MazeType == MazeTypeEnum.square && RectangularGeneration == RectangularGenerationEnum.recursive || RectangularGeneration == RectangularGenerationEnum.better_recursive)
            MaxDepthUi.SetActive(true);
        else if (MazeType == MazeTypeEnum.hexagon && HexagonalGeneration == HexagonalGenerationEnum.better_recursive)
            MaxDepthUi.SetActive(true);
        else
            MaxDepthUi.SetActive(false);

        if (MazeType == MazeTypeEnum.square && RectangularGeneration == RectangularGenerationEnum.recursive)
            VisualModeUi.SetActive(false);
        else
            VisualModeUi.SetActive(true);
    }
    
    public void StartMaze()
	{
        PlayerPrefs.SetInt("Height", Height);
        PlayerPrefs.SetInt("Width", Width);
        PlayerPrefs.SetInt("Radius", Radius);
        PlayerPrefs.SetInt("MaxDepth", RecursiveMaxDepth);
        PlayerPrefs.SetInt("RandomChamber", System.Convert.ToInt32(RecursiveRandomChamber));
        PlayerPrefs.SetInt("MazeType", ((int)MazeType));
        PlayerPrefs.SetInt("HexagonType", ((int)HexagonType));
        PlayerPrefs.SetInt("RectangularGeneration", ((int)RectangularGeneration));
        PlayerPrefs.SetInt("HexagonalGeneration", ((int)HexagonalGeneration));
        PlayerPrefs.SetInt("VisualMode", System.Convert.ToInt32(VisualMode));
        

        SceneManager.LoadScene("SimulationScene");
	}

    public void ChangeMazeType(int value)
	{
        if (value == 0)
            MazeType = MazeTypeEnum.square;
        else if (value == 1)
            MazeType = MazeTypeEnum.hexagon;
	}

    public void ChangeHexagonStyle(int value)
	{
        if (value == 0)
            HexagonType = HexagonTypeEnum.square_flat;
        else if (value == 1)
            HexagonType = HexagonTypeEnum.square_pointy;
        else if (value == 2)
            HexagonType = HexagonTypeEnum.hexagon_flat;
        else if (value == 3)
            HexagonType = HexagonTypeEnum.hexagon_pointy;
    }

    public void ChangerSquareGenMethod(int value)
	{
        if (value == 0)
            RectangularGeneration = RectangularGenerationEnum.backtracker;
        else if (value == 1)
            RectangularGeneration = RectangularGenerationEnum.prims;
        else if (value == 2)
            RectangularGeneration = RectangularGenerationEnum.kruskal;
        else if (value == 3)
            RectangularGeneration = RectangularGenerationEnum.recursive;
        else if (value == 4)
            RectangularGeneration = RectangularGenerationEnum.better_recursive;
    }

    public void ChangerHexagonGenMethod(int value)
    {
        if (value == 0)
            HexagonalGeneration = HexagonalGenerationEnum.backtracker;
        else if (value == 1)
            HexagonalGeneration = HexagonalGenerationEnum.prims;
        else if (value == 2)
            HexagonalGeneration = HexagonalGenerationEnum.kruskal;
        else if (value == 3)
            HexagonalGeneration = HexagonalGenerationEnum.better_recursive;
    }

    public void SetRadius(string value) => Radius = StrToInt(value);
    public void SetHeight(string value) => Height = StrToInt(value);
    public void SetWidth(string value) => Width = StrToInt(value);
    public void SetMaxDepth(string value) => RecursiveMaxDepth = StrToInt(value);
    public void SetRandomRoom(bool value) => RecursiveRandomChamber = value;

	public void SetVisualMode(bool value) => VisualMode = value;

    public void Quit() => Application.Quit();

    private int StrToInt(string str)
	{
        int result;

        int.TryParse(str, out result);
        return result;
	}
}
