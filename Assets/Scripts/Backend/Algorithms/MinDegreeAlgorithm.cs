
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class MinDegreeAlgorithm : Algorithm
{
    public int delta;

    public MinDegreeAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        IEnumerable< int > degrees = this.Graph.Vertices.Select( vert => this.Graph.GetVertexDegree( vert ) );
        this.delta = degrees.Count() > 0 ? degrees.Min() : 0;
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "minimum degree" ] = ( this.delta, typeof ( int ) );
        return result;
    }

    public static int GetHash() => typeof ( MinDegreeAlgorithm ).GetHashCode();

    public override int GetHashCode() => MinDegreeAlgorithm.GetHash();
}
