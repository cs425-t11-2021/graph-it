
// All code developed by Team 11

using System;

[System.Serializable]
public class AveragePathLengthAlgorithm : Algorithm
{
    private float? averagePathLength;

    public AveragePathLengthAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        if (this.Graph.Directed)
            this.CreateError( "Average path length cannot be computed on directed graph." );
        if (this.Graph.Order < 2)
            this.CreateError( "Average path length cannot be computed on graph with order less than 2." );

        this.averagePathLength = 0;

        foreach ( Vertex u in this.Graph.Vertices )
        {
            foreach ( Vertex v in this.Graph.Vertices )
            {
                this.AlgoManager.RunDijkstras( u, v, false );
                this.WaitUntilDijkstrasComplete( u, v );
                float cost = ( float ) this.AlgoManager.GetDijkstras( u, v ).results[ "cost" ].Item1;
                this.averagePathLength += cost;
            }
        }

        this.averagePathLength /= (float) this.Graph.Order * (this.Graph.Order - 1);
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "average path length" ] = ( this.averagePathLength, typeof ( float ) );
        return result;
    }

    private void WaitUntilDijkstrasComplete( Vertex src, Vertex dest )
    {
        this.WaitUntilAlgorithmComplete( DijkstrasAlgorithm.GetHash( src, dest ) );
    }

    public static int GetHash() => typeof ( AveragePathLengthAlgorithm ).GetHashCode();

    public override int GetHashCode() => AveragePathLengthAlgorithm.GetHash();

}
