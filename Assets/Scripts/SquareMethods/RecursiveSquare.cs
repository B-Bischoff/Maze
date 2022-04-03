using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveSquare : MonoBehaviour
{
    [Header("Grid generator reference")]
    public RectangularGrid Grid;

    [Header("Algorithm parameters")]
    private int maxDepth;
    private int width, height;
    private float delay = 0.025f;

    private bool randomChamber;

    [Header("Prefab")]
    public GameObject Ground;
    public GameObject vertWall, horWall;
    private bool _isGenerating;

    void Start()
    {
        width = Grid.Width;
        height = Grid.Height;
        randomChamber = Grid.RandomChamber;
        maxDepth = Grid.MaxDepth;

        if (height <= 0 || width <= 0)
        {
            Debug.Log("Invalid maze length");
            return;
        }

        CreateGround();

        // Setting up inital chamber
        Vector2 bottomLeft = new Vector2(0, 0);
        Vector2 topRight = new Vector2(width, height);

        CreateBorders(bottomLeft, topRight);
        StartCoroutine(CreateMaze(bottomLeft, topRight, maxDepth));
    }

    void CreateGround()
	{
        Vector3 groundPos = new Vector3(width / 2f, 0, height / 2f);
        Vector3 groundScale = new Vector3(width / 10f, 1, height / 10f);
        Ground.transform.localScale = groundScale;
        Instantiate(Ground, groundPos, Quaternion.identity).transform.parent = gameObject.transform;
    }

    void CreateBorders(Vector2 bottomLeft, Vector2 topRight)
	{
        // + .5f is used to create non intersecting walls
        for (int i = (int)bottomLeft.x; i < (int)topRight.x; i++)
        {
            Vector3 bottomPos = new Vector3(i + .5f, 0, bottomLeft.y);
            Vector3 topPos = new Vector3(i + .5f, 0, topRight.y);
            Instantiate(horWall, bottomPos, Quaternion.identity).transform.parent = gameObject.transform;
            Instantiate(horWall, topPos, Quaternion.identity).transform.parent = gameObject.transform;
        }
        for (int i = (int)bottomLeft.y; i < (int)topRight.y; i++)
        {
            Vector3 leftPos = new Vector3(bottomLeft.x, 0, i + .5f);
            Vector3 rightPos = new Vector3(topRight.x, 0, i + .5f);
            Instantiate(vertWall, leftPos, Quaternion.identity).transform.parent = gameObject.transform;
            Instantiate(vertWall, rightPos, Quaternion.identity).transform.parent = gameObject.transform;
        }
    }

    IEnumerator CreateMaze(Vector2 bottomLeft, Vector2 topRight, int depth)
	{
        if (depth == 0)
            yield break;

        if (topRight.x - bottomLeft.x < 3 || topRight.y - bottomLeft.y < 3)
        {
            Debug.Log("Not enough space to create another chamber");
            yield break;
        }

        int x, y;

        if (randomChamber) // Walls at a random position
        {
            x = Random.Range((int)bottomLeft.x + 1, (int)topRight.x);
            y = Random.Range((int)bottomLeft.y + 1, (int)topRight.y);
        }
        else // Walls at the center of the chamber
        {
            x = (int)(bottomLeft.x + (topRight.x - bottomLeft.x) / 2.0);
            y = (int)(bottomLeft.y + (topRight.y - bottomLeft.y) / 2.0);
        }
        
        // Generation 3 passages
        int horPassage = Random.Range((int)bottomLeft.x, (int)topRight.x);
        int vertPassage = Random.Range((int)bottomLeft.y, (int)topRight.y);
        float randomPassagePos = Random.Range(0f, 1f);

        int randomPassage;

        if (randomPassagePos > 0.5f)
		{
            // Generate gap in horizontal wall
            if (horPassage >= x)
                randomPassage = Random.Range((int)bottomLeft.x, x);
            else
                randomPassage = Random.Range(x, (int)topRight.x);
		}
        else
		{
            // Generate gap in vertical wall
            if (vertPassage >= y )
                randomPassage = Random.Range((int)bottomLeft.y, y);
            else
                randomPassage = Random.Range(y, (int)topRight.y);
        }

        for (int i = (int)bottomLeft.y; i < topRight.y; i++) // Creating vertical walls according to passages
        {
            Vector3 pos = new Vector3(x, 0, i + .5f);
            if (i != vertPassage && (i != randomPassage || randomPassagePos > 0.5f))
                Instantiate(vertWall, pos, Quaternion.identity).transform.parent = gameObject.transform;
            yield return new WaitForSeconds(delay);
        }
        for (int i = (int)bottomLeft.x; i < topRight.x; i++) // Creating horizontal walls according to passages
        {
            Vector3 pos = new Vector3(i + .5f, 0, y);
            if (i != horPassage && (i != randomPassage || randomPassagePos < 0.5f))
                Instantiate(horWall, pos, Quaternion.identity).transform.parent = gameObject.transform;
            yield return new WaitForSeconds(delay);
        }

        depth--;

        // Create new corners for each chamber

        //  Top left chamber
        Vector2 chamber1BottomLeft = new Vector2(bottomLeft.x, y);
        Vector2 chamber1TopRight = new Vector2(x, topRight.y);
        StartCoroutine(CreateMaze(chamber1BottomLeft, chamber1TopRight, depth));

        // Bottom left chamber
        Vector2 chamber2BottomLeft = new Vector2(bottomLeft.x, bottomLeft.y);
        Vector2 chamber2TopRight = new Vector2(x, y);
        StartCoroutine(CreateMaze(chamber2BottomLeft, chamber2TopRight, depth));

        // Top right chamber
        Vector2 chamber3BottomLeft = new Vector2(x, y);
        Vector2 chamber3TopRight = new Vector2(topRight.x, topRight.y);
        StartCoroutine(CreateMaze(chamber3BottomLeft, chamber3TopRight, depth));

        // Bottom left chamber
        Vector2 chamber4BottomLeft = new Vector2(x, bottomLeft.y);
        Vector2 chamber4TopRight = new Vector2(topRight.x, y);
        StartCoroutine(CreateMaze(chamber4BottomLeft, chamber4TopRight, depth));

        yield break;
    }        
}
