using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid : SingletonBehavior<Grid>
{
    // Prefab for the grid lines object
    public GameObject girdLinesPrefab;

    // Space between each vertex
    [SerializeField] private float spacing = 0.5f;
    // Number of horizontal and vertical gridline objects to instantiate
    [SerializeField] private int gridLineCount = 20;

    // List of occupied grid points
    private List< (Vector2Int, VertexObj) > occupiedPoints;
    // Array of instantiated horizontal and vertical girdline objects
    private GameObject[] horizontalLines;
    private GameObject[] verticalLines;

    public bool GridEnabled { get; set; }

    private void Awake()
    {
        // Instantiate list of occupied points
        this.occupiedPoints = new List< (Vector2Int, VertexObj) >();

        // Subscribe OnToggleGridSnapping method to the corresponding event in Controller, then run it to get default settings
        SettingsManager.Singleton.OnToggleGridSnapping += OnToggleGridSnapping;
        OnToggleGridSnapping(SettingsManager.Singleton.SnapVerticesToGrid);
    }

    // Method called when the the setting for grid snapping is toggled from Controller
    private void OnToggleGridSnapping(bool enabled)
    {
        GridEnabled = enabled;
    }


    private void Start()
    {
        // Initialize the gridline object arrays
        this.horizontalLines = new GameObject[gridLineCount];
        this.verticalLines = new GameObject[gridLineCount];

        // Instantiate the horizontal and vertical gridlines, add them to the corresponding arrays, and disable them by default
        for (int i = 0; i < this.gridLineCount; i++)
        {
            this.horizontalLines[i] = Instantiate(this.girdLinesPrefab, this.transform);
            this.horizontalLines[i].SetActive(false);
            this.verticalLines[i] = Instantiate(this.girdLinesPrefab, this.transform);
            this.verticalLines[i].SetActive(false);
        }
    }

    // Method for displaying (activating) the gridline objects at the correct locations
    public void DisplayGridLines()
    {
        // Get the world coordinates of the bottom left and top right corners of the camera
        Vector2 lowerLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane));
        Vector2 upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.nearClipPlane));

        // Size in world coordinates of the camera
        Vector2 size = new Vector2(upperRight.x - lowerLeft.x, upperRight.y - lowerLeft.y);

        // Find the grid point associated with the lower left corner
        Vector2Int start = WorldToGrid(lowerLeft - size / 2);
        // Activate vertical lines separated by the grid spacing until the right edge of the camera has been exceeded
        for (int i = 0; i < this.gridLineCount; i++)
        {
            float lineX = (start.x + i) * this.spacing;
            if (lineX > upperRight.x)
            {
                // The right edge of the camera has been exceeded
                break;
            }
            LineRenderer lr = this.verticalLines[i].GetComponent<LineRenderer>();
            // Make the lines render from top of the camera to bottom of the camera
            lr.SetPositions(new Vector3[] {new Vector3(lineX, lowerLeft.y, 1), new Vector3(lineX, upperRight.y, 1) });
            // Set the line width to be a ratio of the current camera size to ensure the gridline width is constant across camera sizes
            lr.startWidth = Camera.main.orthographicSize / 160f;
            lr.endWidth = Camera.main.orthographicSize / 160f;
            this.verticalLines[i].SetActive(true);
        }

        // Activate horizontal lines separated by the grid spacing until the top edge of the camera has been exceeded
        for (int i = 0; i < this.gridLineCount; i++)
        {
            float lineY = (start.y + i) * this.spacing;
            if (lineY > upperRight.y * 2)
            {
                // The top edge of the camera has been exceeded
                break;
            }
            LineRenderer lr = this.horizontalLines[i].GetComponent<LineRenderer>();
            // Make the lines render from left of the camera to right of the camera
            lr.SetPositions(new Vector3[] { new Vector3(lowerLeft.x, lineY, 1), new Vector3(upperRight.x, lineY, 1) });
            // Set the line width to be a ratio of the current camera size to ensure the gridline width is constant across camera sizes
            lr.startWidth = Camera.main.orthographicSize / 160f;
            lr.endWidth = Camera.main.orthographicSize / 160f;
            this.horizontalLines[i].SetActive(true);
        }
    }

    // Method for hiding (deactivating) the gridlines
    public void HideGridLines()
    {
        Array.ForEach(this.horizontalLines, line => line.SetActive(false));
        Array.ForEach(this.verticalLines, line => line.SetActive(false));
    }

    // Find the best point in the grid of a given vertex object
    public Vector2 FindClosestGridPosition(VertexObj vertex)
    {
        // Get the grid point closest to the position of the vertex object
        Vector2Int bestPoint = WorldToGrid(vertex.transform.position);
        // If the closest point is occupied by another vertex object, find the nearest unoccupied point
        if (this.occupiedPoints.Any(x => x.Item1 == bestPoint))
        {
            bestPoint = FindClosestUnoccupiedPoint(bestPoint);
        }

        // Once the best point has been found, add the vertex and the point to the list of occupied points
        this.occupiedPoints.Add( (bestPoint, vertex) );
        // Return the world coordinate of the found point
        return GridToWorld(bestPoint);
    }

    // Function to find the cloest unoccupied grid point given a starting point
    public Vector2Int FindClosestUnoccupiedPoint(Vector2Int start)
    {
        int radius = 0;
        // NOTE: Arbitrary search limit of 10
        while (radius < 10)
        {
            radius++;
            // Iterate through each of the 9 adjacent points to find an unoccupied point
            // TODO: Analyze later, this code might be incorrect?
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2Int newPoint = start + new Vector2Int(i * radius, j * radius);
                    if (!this.occupiedPoints.Any(x => x.Item1 == newPoint))
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
        this.occupiedPoints.RemoveAll(x => x.Item2 == vertex);
    }

    // Find the closest grid point corresponding to a world coordinate
    public Vector2Int WorldToGrid(Vector2 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x / this.spacing), Mathf.RoundToInt(position.y / this.spacing));
    }

    // Find the world coordinate of a grid point
    public Vector2 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector2(gridPosition.x * spacing, gridPosition.y * this.spacing);
    }

    // Set all grid points as unoccupied
    public void ClearGrid()
    {
        this.occupiedPoints = new List<(Vector2Int, VertexObj)>();
    }
}
