
// All code developed by Team 11

using System;

[System.Serializable]
public class AveragePathLengthAlgorithm : Algorithm
{
    public float? AveragePathLength { get; private set; }

    public AveragePathLengthAlgorithm( AlgorithmManager algoManager, bool display ) : base( algoManager ) { }

    public override void Run()
    {
        if (this.Graph.Directed)
        {
            this.AveragePathLength = null;
            return;
        }

        this.AveragePathLength = 0;

        if (this.Graph.Order < 2)
        {
            return;
        }

        foreach ( Vertex u in this.Graph.Vertices )
        {
            foreach ( Vertex v in this.Graph.Vertices )
            {
                this.AlgoManager.RunDijkstras( u, v, false );
                this.WaitUntilDijkstrasComplete( u, v );
                float cost = ( float ) this.AlgoManager.GetDijkstrasCost( u, v );
                this.AveragePathLength += cost;
            }
        }

        this.AveragePathLength /= (float) this.Graph.Order * (this.Graph.Order - 1);
    }

    private void WaitUntilDijkstrasComplete( Vertex src, Vertex dest )
    {
        this.WaitUntilAlgorithmComplete( DijkstrasAlgorithm.GetHash( src, dest ) );
    }

    public static int GetHash() => typeof ( AveragePathLengthAlgorithm ).GetHashCode();

    public override int GetHashCode() => AveragePathLengthAlgorithm.GetHash();

}
