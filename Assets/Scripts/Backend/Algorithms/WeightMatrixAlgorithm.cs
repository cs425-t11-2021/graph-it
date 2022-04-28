
// All code developed by Team 11

using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class WeightMatrixAlgorithm : Algorithm
{
    // weight matrix, rows and columns parallel to this.Graph.Vertices
    private float[,] matrix;

    public WeightMatrixAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        this.matrix = new float[ this.Graph.Order, this.Graph.Order ];
        for ( int i = 0; i < this.Graph.Order; ++i )
        {
            for ( int j = 0; j < this.Graph.Order; ++j )
                this.matrix[ i, j ] = this.Graph.IsAdjacentDirected( this.Graph.Vertices[ i ], this.Graph.Vertices[ j ] ) ? this.Graph[ this.Graph.Vertices[ i ], this.Graph.Vertices[ j ] ].Weight : Single.PositiveInfinity;
        }
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "weight matrix" ] = ( this.matrix, typeof ( float[,] ) );
        return result;
    }

    public static int GetHash() => typeof ( WeightMatrixAlgorithm ).GetHashCode();

    public override int GetHashCode() => WeightMatrixAlgorithm.GetHash();
}
