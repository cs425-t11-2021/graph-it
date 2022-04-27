
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class AdjacencyMatrixAlgorithm : Algorithm
{
    // adjacency matrix, rows and columns parallel to this.Graph.Vertices
    private bool[,] matrix;

    public AdjacencyMatrixAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        this.matrix = new bool[ this.Graph.Order, this.Graph.Order ];
        for ( int i = 0; i < this.Graph.Order; ++i )
        {
            for ( int j = 0; j < this.Graph.Order; ++j )
                this.matrix[ i, j ] = this.Graph.IsAdjacentDirected( this.Graph.Vertices[ i ], this.Graph.Vertices[ j ] );
        }
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "adjacency matrix" ] = ( this.matrix, typeof ( bool[,] ) );
        return result;
    }

    public static int GetHash() => typeof ( AdjacencyMatrixAlgorithm ).GetHashCode();

    public override int GetHashCode() => AdjacencyMatrixAlgorithm.GetHash();
}
