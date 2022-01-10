using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid : MonoBehaviour
{
    // Singleton
    public static Grid singleton;

    // Prefab for the grid lines object
    public GameObject p_gridLines;

    // Whether or not vertices should snap to grid
    public bool enableGrid = false;
    // Space between each vertex
    public float spacing = 0.5f;
    // Number of horizontal and vertical gridlines to instantiate
    public int gridLineCount = 20;

    // List of occupied grid points
    public List< (Vector2Int, VertexObj) > occupiedPoints;
    // Array of horizontal and vertical girdline objects
    private GameObject[] horizontalLines;
    private GameObject[] verticalLines;

    private void Awake()
    {
        // Singleton pattern setup
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Debug.LogError("[Grid] Singleton pattern violation");
            Destroy(this);
            return;
        }

        // Instantiate list of occupied points
        occupiedPoints = new List< (Vector2Int, VertexObj) >();
    }

    
    private void Start()
    {
        horizontalLines = new GameObject[gridLineCount];
        verticalLines = new GameObject[gridLineCount];

        for (int i = 0; i < gridLineCount; i++)
        {
            horizontalLines[i] = Instantiate(p_gridLines, this.transform);
            horizontalLines[i].SetActive(false);
            verticalLines[i] = Instantiate(p_gridLines, this.transform);
            verticalLines[i].SetActive(false);
        }

        HideGridLines();
    }

    public void DisplayGridLines()
    {
        Vector2 lowerLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane));
        Vector2 upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.nearClipPlane));

        Vector2Int start = WorldToGrid(lowerLeft);
        for (int i = 0; i < gridLineCount; i++)
        {
            float lineX = (start.x + i) * spacing;
            if (lineX > upperRight.x)
            {
                break;
            }
            LineRenderer lr = verticalLines[i].GetComponent<LineRenderer>();
            lr.SetPositions(new Vector3[]{new Vector3(lineX, lowerLeft.y, 1), new Vector3(lineX, upperRight.y, 1) });
            lr.startWidth = Camera.main.orthographicSize / 160f;
            lr.endWidth = Camera.main.orthographicSize / 160f;
            verticalLines[i].SetActive(true);
        }

        for (int i = 0; i < gridLineCount; i++)
        {
            float lineY = (start.y + i) * spacing;
            if (lineY > upperRight.y)
            {
                break;
            }
            LineRenderer lr = horizontalLines[i].GetComponent<LineRenderer>();
            lr.SetPositions(new Vector3[] { new Vector3(lowerLeft.x, lineY, 1), new Vector3(upperRight.x, lineY, 1) });
            lr.startWidth = Camera.main.orthographicSize / 160f;
            lr.endWidth = Camera.main.orthographicSize / 160f;
            horizontalLines[i].SetActive(true);
        }
    }

    public void HideGridLines()
    {
        foreach (GameObject line in horizontalLines)
        {
            line.SetActive(false);
        }
        foreach (GameObject line in verticalLines)
        {
            line.SetActive(false);
        }
    }

    // Find the best point in the grid of a given vertex object
    public Vector2 FindClosestGridPosition(VertexObj vertex)
    {
        Vector2Int bestPoint = WorldToGrid(vertex.transform.position);
        if (occupiedPoints.Any(x => x.Item1 == bestPoint))
        {
            bestPoint = FindClosestUnoccupiedPoint(bestPoint);
        }

        occupiedPoints.Add( (bestPoint, vertex) );
        return GridToWorld(bestPoint);
    }

    // Function to find the cloest unoccupied grid point given a starting point
    public Vector2Int FindClosestUnoccupiedPoint(Vector2Int start)
    {
        int radius = 0;
        while (radius < 10)
        {
            radius++;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2Int newPoint = start + new Vector2Int(i * radius, j * radius);
                    if (!occupiedPoints.Any(x => x.Item1 == newPoint))
                    {
                        return newPoint;
                    }
                }
            }
        }
        return start;
    }

    // Remove vertex from list of occupied points
    public void RemoveFromOccupied(VertexObj vertex)
    {
        occupiedPoints.RemoveAll(x => x.Item2 == vertex);
    }

    // Find the closest grid point corresponding to a world coordinate
    public Vector2Int WorldToGrid(Vector2 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x / spacing), Mathf.RoundToInt(position.y / spacing));
    }

    // Find the world coordinate of a grid point
    public Vector2 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector2(gridPosition.x * spacing, gridPosition.y * spacing);
    }

    // Set all grid points as unoccupied
    public void ClearGrid()
    {
        occupiedPoints = new List<(Vector2Int, VertexObj)>();
    }
}
