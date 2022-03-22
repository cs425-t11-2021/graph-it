
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class MaxDegreeAlgorithm : Algorithm
{
    public int MaxDegree { get; private set; }

    public MaxDegreeAlgorithm( AlgorithmManager algoManager ) : base( algoManager ) { }

    public override void Run()
    {
        IEnumerable< int > degrees = this.Graph.Vertices.Select( vert => this.Graph.GetVertexDegree( vert ) );
        this.MaxDegree = degrees.Count() > 0 ? degrees.Max() : 0;
    }

    public static int GetHash() => typeof ( MaxDegreeAlgorithm ).GetHashCode();

    public override int GetHashCode() => MaxDegreeAlgorithm.GetHash();
}
