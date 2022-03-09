
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class MaxDegreeAlgorithm : Algorithm
{
    public int MaxDegree { get; private set; }

    public MaxDegreeAlgorithm( Graph graph, CancellationToken token, Action updateUI, Action updateCalc, Action< Algorithm > markRunning, Action< Algorithm > markComplete, Action< Algorithm > unmarkRunning ) : base( graph, token, updateUI, updateCalc, markRunning, markComplete, unmarkRunning ) { }

    public override void Run()
    {
        IEnumerable< int > degrees = this.Graph.Vertices.Select( vert => this.Graph.GetVertexDegree( vert ) );
        this.MaxDegree = degrees.Count() > 0 ? degrees.Max() : 0;

        ChromaticAlgorithm.SetMaxDegree( this.Graph, this.MaxDegree );
    }

    protected override void Kill()
    {
        base.Kill();
        ChromaticAlgorithm.ClearMaxDegrees( this.Graph );
    }

    public static int GetHash() => typeof ( MaxDegreeAlgorithm ).GetHashCode();

    public override int GetHashCode() => MaxDegreeAlgorithm.GetHash();
}
