
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class MinDegreeAlgorithm : Algorithm
{
    public int MinDegree { get; private set; }

    public MinDegreeAlgorithm( AlgorithmManager algoManager ) : base( algoManager ) { }

    public override void Run()
    {
        IEnumerable< int > degrees = this.Graph.Vertices.Select( vert => this.Graph.GetVertexDegree( vert ) );
        this.MinDegree = degrees.Count() > 0 ? degrees.Min() : 0;
    }

    public static int GetHash() => typeof ( MinDegreeAlgorithm ).GetHashCode();

    public override int GetHashCode() => MinDegreeAlgorithm.GetHash();
}
