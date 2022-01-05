using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid : MonoBehaviour
{
    // Singleton
    public static Grid singleton;

    // Prefab for the marker debug object
    public GameObject p_marker;

    // Whether or not vertices should snap to grid
    public bool enableGrid = false;
    // Space between each vertex
    public float spacing = 0.5f;

    // List of occupied grid points
    public List< (Vector2Int, VertexObj) > occupiedPoints;

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

        occupiedPoints = new List< (Vector2Int, VertexObj) >();
    }

    // Debug: Generate markers for each of the grid point in 15 x 15
    private void Start()
    {
        for (int i = -10; i <= 10; i++)
        {
            for (int j = -10; j <= 10; j++)
            {
                Instantiate(p_marker, new Vector3(i * spacing, j * spacing, 0), Quaternion.identity);
            }
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
}
