//All code developed by Team 11
using System;
using System.Collections;
using System.Collections.Concurrent;
// Utilitizes 3rd party TableUI API from the Unity Asset Store

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.TableUI;

// Work in progress feature for displaying an adjacency matrix for the graph on screen. Not currently working as intended.
public class AdjacencyMatrix : MonoBehaviour
{
    private TableUI table;
    private bool matrixEnabled = false;

    private void Awake()
    {
        this.table = GetComponentInChildren<TableUI>(true);
        this.table.gameObject.SetActive(false);
        this.table.HeaderColor = this.table.MainColor;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Backslash))
        {
            if (!matrixEnabled)
            {
                matrixEnabled = true;
                ShowAdjacencyMatrix();
            }
        }
        else if (matrixEnabled)
        {
            matrixEnabled = false;
            HideAdjacencyMatrix();
        }
    }

    public void ShowAdjacencyMatrix()
    {
        this.table.Columns = Controller.Singleton.Graph.Order + 1;
        this.table.Rows = Controller.Singleton.Graph.Order + 1;

        for (int i = 0; i < this.table.Columns; i++)
        {
            this.table.UpdateColumnWidth(120f, i);
        }
        
        for (int i = 0; i < this.table.Rows; i++)
        {
            this.table.UpdateRowHeight(120f, i);
        }

        Dictionary<(Vertex, Vertex), Edge> adjacency = Controller.Singleton.Graph.Adjacency;
        List<Vertex> vertices = Controller.Singleton.Graph.Vertices;

        this.table.GetCell(0, 0).text = "";
        for (int i = 1; i < this.table.Rows; i++)
        {
            this.table.GetCell(0, i).text = vertices[i - 1].Label;
            this.table.GetCell(i, 0).text = vertices[i - 1].Label;
        }
        
        for (int i = 1; i < this.table.Rows; i++)
        {
            for (int j = 1; j < this.table.Columns; j++)
            {
                Edge edge = Controller.Singleton.Graph[vertices[i - 1], vertices[j - 1]];
                this.table.GetCell(i, j).text = (edge != null ? edge.Weight.ToString() : "0");
            }
        }
        
        this.table.gameObject.SetActive(true);
    }

    public void HideAdjacencyMatrix()
    {
        this.table.gameObject.SetActive(false);
    }
}
