
// All code developed by Team 11

using System;

[System.Serializable]
public class DiameterAlgorithm : Algorithm
{
    public float diameter;

    public DiameterAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        if (this.Graph.Directed)
            this.CreateError( "Diameter cannot be computed on directed graph." );

        this.diameter = 0;

        foreach ( Vertex u in this.Graph.Vertices )
        {
            foreach ( Vertex v in this.Graph.Vertices )
            {
                this.AlgoManager.RunDijkstras( u, v, false );
                this.WaitUntilDijkstrasComplete( u, v );
                float cost = ( float ) this.AlgoManager.GetDijkstras( u, v ).results[ "cost" ].Item1;
                if (cost > this.diameter)
                    this.diameter = cost;
            }
        }
    }

    public override AlgorithmResult GetResult()
    {
        if ( this.error )
            return this.GetErrorResult();
        if ( this.running )
            return this.GetRunningResult();
        AlgorithmResult result = new AlgorithmResult( AlgorithmResultType.SUCCESS );
        result.results[ "diameter" ] = ( this.diameter, typeof ( float ) );
        return result;
    }

    private void WaitUntilDijkstrasComplete( Vertex src, Vertex dest )
    {
        this.WaitUntilAlgorithmComplete( DijkstrasAlgorithm.GetHash( src, dest ) );
    }

    public static int GetHash() => typeof ( DiameterAlgorithm ).GetHashCode();

    public override int GetHashCode() => DiameterAlgorithm.GetHash();

}