
// All code developed by Team 11

using System;

[System.Serializable]
public class RadiusAlgorithm : Algorithm
{
    private float radius;

    public RadiusAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        if (this.Graph.Directed)
            this.CreateError( "Radius cannot be computed on directed graph." );

        if (this.Graph.Order == 0)
        {
            this.radius = 0;
            return;
        }

        this.radius = float.PositiveInfinity;
        foreach ( Vertex u in this.Graph.Vertices )
        {
            float max_dist = 0;
            foreach ( Vertex v in this.Graph.Vertices )
            {
                this.AlgoManager.RunDijkstras( u, v, false );
                this.WaitUntilDijkstrasComplete( u, v );
                float cost = ( float ) this.AlgoManager.GetDijkstras( u, v ).results[ "cost" ].Item1;
                if (cost > max_dist)
                {
                    max_dist = cost;
                }
            }

            if (max_dist < radius)
            {
                this.radius = max_dist;
            }
        }
    }

    private void WaitUntilDijkstrasComplete( Vertex src, Vertex dest )
    {
        this.WaitUntilAlgorithmComplete( DijkstrasAlgorithm.GetHash( src, dest ) );
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "radius" ] = ( this.radius, typeof ( float ) );
        return result;
    }

    public static int GetHash() => typeof ( RadiusAlgorithm ).GetHashCode();

    public override int GetHashCode() => RadiusAlgorithm.GetHash();

}