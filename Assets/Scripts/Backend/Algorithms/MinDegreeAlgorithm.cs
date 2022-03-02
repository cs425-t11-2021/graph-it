
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class MinDegreeAlgorithm : Algorithm
{
    public int MinDegree { get; private set; }

    public MinDegreeAlgorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        Dictionary< Vertex, int >.ValueCollection degrees = this.GetDegrees().Values;
        this.MinDegree = degrees.Count > 0 ? degrees.Min() : 0;
    }

    private Dictionary< Vertex, int > GetDegrees()
    {
        Dictionary< Vertex, int > degrees = new Dictionary< Vertex, int >();
        foreach ( Vertex vert in this.Graph.Vertices )
            degrees[ vert ] = this.Graph.GetIncidentEdges( vert ).Count;
        return degrees;
    }

    public static int GetHash() => typeof ( MinDegreeAlgorithm ).GetHashCode();

    public override int GetHashCode() => MinDegreeAlgorithm.GetHash();
}
