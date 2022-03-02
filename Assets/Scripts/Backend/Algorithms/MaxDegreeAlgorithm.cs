
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class MaxDegreeAlgorithm : Algorithm
{
    public int MaxDegree { get; private set; }

    public MaxDegreeAlgorithm( Graph graph, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        Dictionary< Vertex, int >.ValueCollection degrees = this.GetDegrees().Values;
        this.MaxDegree = degrees.Count > 0 ? degrees.Max() : 0;
    }

    private Dictionary< Vertex, int > GetDegrees()
    {
        Dictionary< Vertex, int > degrees = new Dictionary< Vertex, int >();
        foreach ( Vertex vert in this.Graph.Vertices )
            degrees[ vert ] = this.Graph.GetIncidentEdges( vert ).Count;
        return degrees;
    }

    public static int GetHash() => typeof ( MaxDegreeAlgorithm ).GetHashCode();

    public override int GetHashCode() => MaxDegreeAlgorithm.GetHash();
}
