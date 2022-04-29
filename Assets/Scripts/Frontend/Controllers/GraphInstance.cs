//All code developed by Team 11

using System.Collections.Generic;
using UnityEngine;

// Class which stores a instance of graph datastructure along with its associated Unity objects
public class GraphInstance
{
    public uint id;
    public Graph graph;
    public Transform container;
    public List<VertexObj> vertexObjs;
    public List<EdgeObj> edgeObjs;
    public AlgorithmManager algorithmManager;

    public GraphInstance(Transform container, uint id, AlgorithmManager algoMan, Graph existingGraph = null)
    {
        this.id = id;
        this.graph = existingGraph ?? new Graph();
        this.vertexObjs = new List<VertexObj>();
        this.edgeObjs = new List<EdgeObj>();
        this.container = container;
        this.algorithmManager = algoMan;
    }
}