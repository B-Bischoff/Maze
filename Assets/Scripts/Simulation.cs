using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Maze_enums;

public class Simulation : MonoBehaviour
{
    public MazeManager MazeManager;

    public TextMeshProUGUI DelayText;
    public TextMeshProUGUI PauseText;
    public TextMeshProUGUI InfoText;

    private bool Paused = false;

	private void Start()
	{
        PrintInfos();
        PauseText.text = "PAUSE";
        SetDelay(1f);
	}

    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuScene");
    }

    public void SetDelay(float value)
    {
        DelayText.text = "Delay : " + value.ToString();
        MazeManager.Delay = value;
    }

    public void TogglePause()
	{
        if (Paused)
        {
            Time.timeScale = 1;
            PauseText.text = "PAUSE";
        }
        else
        {
            Time.timeScale = 0;
            PauseText.text = "RESUME";
        }
        Paused = !Paused;
	}

    void PrintInfos()
	{
        string str;

        if (MazeManager.MazeType == MazeTypeEnum.square)
		{
            str = MazeManager.RectangularGeneration.ToString();
            str += " | " + MazeManager.Height + " x " + MazeManager.Width;
            if (MazeManager.RectangularGeneration == RectangularGenerationEnum.recursive || MazeManager.RectangularGeneration == RectangularGenerationEnum.better_recursive)
                str += " | max depth : " + MazeManager.RecursiveMaxDepth;
        }
        else
		{
            str = MazeManager.HexagonalGeneration.ToString();
            if (MazeManager.HexagonType == HexagonTypeEnum.square_flat || MazeManager.HexagonType == HexagonTypeEnum.square_pointy)
                str += " | " + MazeManager.Height + " x " + MazeManager.Width;
            else
                str += " | radius : " + MazeManager.Radius;
            if (MazeManager.HexagonalGeneration == HexagonalGenerationEnum.better_recursive)
                str += " | max depth : " + MazeManager.RecursiveMaxDepth;
        }

        InfoText.text = str;
	}
}
